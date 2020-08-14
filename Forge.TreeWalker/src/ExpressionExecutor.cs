//-----------------------------------------------------------------------
// <copyright file="ExpressionExecutor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The ExpressionExecutor class.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.TreeWalker
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;

    /// <summary>
    /// The ExpressionExecutor dynamically compiles code and executes it using Roslyn.
    /// </summary>
    public class ExpressionExecutor
    {
        /// <summary>
        /// Code that Forge uses to prime the parentScript on initialization.
        /// Evaluating any random code moves roughly 600ms from the first expression to initializing the parentScript.
        /// The first expression still takes roughly 100ms when tested.
        /// The code block is arbitrary, though I did find in testing that the first expression ran quicker when the logic was similar to the parentScriptCode.
        /// </summary>
        public static string ParentScriptCode = "(1+1).ToString()";

        /// <summary>
        /// The parentScriptTask kicks off initializing the Roslyn parentScript asynchronously, allowing ExpressionExecutor to construct very quickly.
        /// Initializing the Roslyn parentScript takes about 2 seconds. This time is saved if the application takes time to initialize before the first WalkTree call.
        /// </summary>
        public Task parentScriptTask { get; private set; }

        /// <summary>
        /// The parentScript that, upon initialization, gets Created, RunAsync, and added to the ScriptCache.
        /// All expressions that get Executed are continued from the parentScript, avoiding additional compiles.
        /// 
        /// Before improvement - each unique expression would cost 25MB, take 2 seconds on first execution, then 0ms on further executions.
        /// After improvement - only the parentScript costs 25MB and takes 2 seconds. Each unique expression costs < 0.5MB, takes 15ms on first execution, then 0ms on further executions.
        /// </summary>
        private Script<object> parentScript;

        /// <summary>
        /// List of external type dependencies needed to compile expressions.
        /// </summary>
        private List<Type> dependencies;

        /// <summary>
        /// The ScriptCache holds Roslyn Scripts that are created using parentScript.ContinueWith.
        /// This saves on memory and time since these continued Scripts use the already compiled parentScript as a base.
        /// The parentScript gets asynchronously compiled, ran, and cached on initialization.
        /// </summary>
        private ConcurrentDictionary<string, Script<object>> scriptCache;

        /// <summary>
        /// Global parameters passed to Roslyn scripts that can be referenced inside expressions.
        /// </summary>
        private CodeGenInputParams parameters;

        /// <summary>
        /// Instantiates the ExpressionExecutor class with objects that can be referenced in the schema.
        /// </summary>
        /// <param name="session">The tree session.</param>
        /// <param name="userContext">The dynamic user context.</param>
        /// <param name="dependencies">Type dependencies required to compile the schema. Can be null if no external dependencies required.</param>
        /// <param name="scriptCache">Script cache used to cache and re-use compiled Roslyn scripts.</param>
        /// <param name="treeInput">The dynamic TreeInput object for this tree walking session.</param>
        public ExpressionExecutor(ITreeSession session, object userContext, List<Type> dependencies, ConcurrentDictionary<string, Script<object>> scriptCache, object treeInput)
        {
            this.dependencies = dependencies;
            this.parameters = new CodeGenInputParams
            {
                UserContext = userContext,
                Session = session,
                TreeInput = treeInput
            };

            this.scriptCache = scriptCache ?? new ConcurrentDictionary<string, Script<object>>();
            this.Initialize();
        }

        /// <summary>
        /// Instantiates the ExpressionExecutor class with objects that can be referenced in the schema.
        /// </summary>
        /// <param name="session">The tree session.</param>
        /// <param name="userContext">The dynamic user context.</param>
        /// <param name="dependencies">Type dependencies required to compile the schema. Can be null if no external dependencies required.</param>
        /// <param name="scriptCache">Script cache used to cache and re-use compiled Roslyn scripts.</param>
        public ExpressionExecutor(ITreeSession session, object userContext, List<Type> dependencies, ConcurrentDictionary<string, Script<object>> scriptCache)
            : this(session, userContext, dependencies, scriptCache, null)
        {
        }

        /// <summary>
        /// Instantiates the ExpressionExecutor class with objects that can be referenced in the schema.
        /// </summary>
        /// <param name="session">The tree session.</param>
        /// <param name="userContext">The dynamic user context.</param>
        /// <param name="dependencies">Type dependencies required to compile the schema. Can be null if no external dependencies required.</param>
        public ExpressionExecutor(ITreeSession session, object userContext, List<Type> dependencies)
            : this(session, userContext, dependencies, new ConcurrentDictionary<string, Script<object>>())
        {
        }

        /// <summary>
        /// Instantiates the ExpressionExecutor class with objects that can be referenced in the schema.
        /// </summary>
        /// <param name="session">The tree session.</param>
        /// <param name="userContext">The dynamic user context.</param>
        public ExpressionExecutor(ITreeSession session, object userContext)
            : this(session, userContext, null)
        {
        }

        /// <summary>
        /// Executes the given expression and returns the result as the given generic type.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>The T value of the evaluated code.</returns>
        public async Task<T> Execute<T>(string expression)
        {
            await this.parentScriptTask;

            Script<object> expressionScript = this.scriptCache.GetOrAdd(
                expression,
                (key) => this.parentScript.ContinueWith(string.Format("return {0};", expression)));

            // Execute script and return the result.
            // Parse Enum types explicitly since they cannot be casted directly.
            object result = (await expressionScript.RunAsync(this.parameters).ConfigureAwait(false)).ReturnValue;
            return typeof(T).IsEnum ? (T)Enum.Parse(typeof(T), result.ToString()) : (T)result;
        }

        /// <summary>
        /// Initializes Roslyn script options with all the required assemblies, references, and external dependencies.
        /// </summary>
        private void Initialize()
        {
            if (this.scriptCache.TryGetValue(ParentScriptCode, out this.parentScript))
            {
                // The parentScript is already initialized.
                this.parentScriptTask = Task.CompletedTask;
                return;
            }

            this.parentScriptTask = Task.Run(async () =>
            {
                ScriptOptions scriptOptions = ScriptOptions.Default.WithMetadataResolver(new MissingResolver());

                // Add references to required assemblies.
                Assembly mscorlib = typeof(object).Assembly;
                Assembly systemCore = typeof(System.Linq.Enumerable).Assembly;
                Assembly cSharpAssembly = typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly;
                scriptOptions = scriptOptions.AddReferences(mscorlib, systemCore, cSharpAssembly);

                // Add required namespaces.
                scriptOptions = scriptOptions.AddImports(
                    "System",
                    "System.Threading.Tasks");

                string systemCoreAssemblyName = mscorlib.GetName().Name;

                // Add external dependencies.
                if (this.dependencies != null)
                {
                    foreach (Type type in this.dependencies)
                    {
                        string fullAssemblyName = type.Assembly.GetName().Name;

                        // While adding the reference again is okay, we can not AddImports for systemCoreAssembly.
                        if (fullAssemblyName == systemCoreAssemblyName)
                        {
                            continue;
                        }

                        scriptOptions = scriptOptions.AddReferences(type.Assembly).AddImports(type.Namespace);
                    }
                }

                // Create the parentScript and add it to scriptCache. Execute the parentScript so Roslyn is primed to evaluate further expressions.
                this.parentScript = this.scriptCache.GetOrAdd(
                    ParentScriptCode,
                    (key) => CSharpScript.Create<object>(ParentScriptCode, scriptOptions, typeof(CodeGenInputParams)));

                await this.parentScript.RunAsync(this.parameters).ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Used for testing if ScriptCache contains the expression key.
        /// </summary>
        /// <param name="expression">The expression key.</param>
        /// <returns>True if the expression key exists, otherwise false.</returns>
        public bool ScriptCacheContainsKey(string expression)
        {
            return this.scriptCache.ContainsKey(expression);
        }

        /// <summary>
        /// This class defines the global parameter that will be passed into the Roslyn expression evaluator.
        /// 
        /// TODO: When Creating a Roslyn Script, the entire Assembly that the passed in GlobalsType resides in gets loaded.
        ///       An optimization would be to move this CodeGenInputParams to its own project/assembly.
        /// </summary>
        public class CodeGenInputParams
        {
            /// <summary>
            /// The dynamic UserContext object that holds properties and methods that can be referenced in the schema.
            /// </summary>
            public dynamic UserContext { get; set; }

            /// <summary>
            /// The ITreeSession interface that holds accessor methods into the forgeState dictionary that can be referenced in the schema.
            /// </summary>
            public ITreeSession Session { get; set; }

            /// <summary>
            /// The dynamic TreeInput object for this tree walking session.
            /// This is passed in to the root/parent session by the App.
            /// For Subroutines, this is evaluated from the SubroutineInput on the schema.
            /// </summary>
            public dynamic TreeInput { get; set; }
        }

        /// <summary>
        /// The MissingResolve class is used to reduce the memory consumption of Roslyn.
        /// This is accomplished by not loading missing references.
        /// The end result is that Creating/Compiling a Script takes 25MB instead of 75MB.
        /// </summary>
        private class MissingResolver : Microsoft.CodeAnalysis.MetadataReferenceResolver
        {
            public override bool Equals(object other)
            {
                throw new NotImplementedException();
            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }

            public override bool ResolveMissingAssemblies => false;

            public override ImmutableArray<PortableExecutableReference> ResolveReference(string reference, string baseFilePath, MetadataReferenceProperties properties)
            {
                throw new NotImplementedException();
            }
        }
    }
}