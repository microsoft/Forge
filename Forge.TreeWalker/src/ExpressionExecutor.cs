//-----------------------------------------------------------------------
// <copyright file="ExpressionExecutor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The ExpressionExecutor class.
// </summary>
//-----------------------------------------------------------------------

namespace Forge.TreeWalker
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;

    /// <summary>
    /// The ExpressionExecutor dynamically compiles code and executes it using Roslyn.
    /// </summary>
    public class ExpressionExecutor
    {
        /// <summary>
        /// List of external type dependencies needed to compile expressions.
        /// </summary>
        private List<Type> dependencies;

        /// <summary>
        /// Script cache used to cache and re-use compiled Roslyn scripts.
        /// </summary>
        private ConcurrentDictionary<string, Script<object>> scriptCache;

        /// <summary>
        /// Roslyn script options.
        /// </summary>
        private ScriptOptions scriptOptions;

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
        /// <param name="treeInput">The dynamic TreeInput object for this tree walking session.</param>
        public ExpressionExecutor(ITreeSession session, object userContext, List<Type> dependencies, object treeInput)
            : this(session, userContext, dependencies, new ConcurrentDictionary<string, Script<object>>(), treeInput)
        {
        }

        /// <summary>
        /// Executes the given expression and returns the result as the given generic type.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>The T value of the evaluated code.</returns>
        public async Task<T> Execute<T>(string expression)
        {
            var script = this.scriptCache.GetOrAdd(
                expression,
                (key) => CSharpScript.Create<object>(
                             string.Format("return {0};", expression),
                             this.scriptOptions,
                             typeof(CodeGenInputParams)));

            return (T)(await script.RunAsync(this.parameters).ConfigureAwait(false)).ReturnValue;
        }

        /// <summary>
        /// Initializes Roslyn script options with all the required assemblies, references, and external dependencies.
        /// </summary>
        private void Initialize()
        {
            this.scriptOptions = ScriptOptions.Default;

            // Add references to required assemblies.
            Assembly mscorlib = typeof(object).Assembly;
            Assembly cSharpAssembly = typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly;
            this.scriptOptions = this.scriptOptions.AddReferences(mscorlib, cSharpAssembly);

            // Add required namespaces.
            this.scriptOptions = this.scriptOptions.AddImports("System");
            this.scriptOptions = this.scriptOptions.AddImports("System.Threading.Tasks");

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

                    this.scriptOptions = this.scriptOptions.AddReferences(type.Assembly).AddImports(type.Namespace);
                }
            }
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
        /// This class defines the global parameter that will be
        /// passed into the Roslyn expression evaluator.
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
    }
}