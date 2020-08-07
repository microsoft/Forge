//-----------------------------------------------------------------------
// <copyright file="TreeWalkerParameters.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The TreeWalkerParameters class.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.TreeWalker
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Scripting;
    using Microsoft.Forge.DataContracts;

    /// <summary>
    /// The TreeWalkerParameters class contains the required and optional properties used by the TreeWalkerSession.
    /// </summary>
    public class TreeWalkerParameters
    {
        #region Required Properties

        /// <summary>
        /// The unique identifier for this session.
        /// </summary>
        public Guid SessionId { get; private set; }

        /// <summary>
        /// The string representation of the JSON schema.
        /// Note: Either ForgeTree or JsonSchema must be used to construct a TreeWalkerParameters. The other property should be null.
        /// </summary>
        public string JsonSchema { get; private set; }

        /// <summary>
        /// The ForgeTree for this session.
        /// Note: Either ForgeTree or JsonSchema must be used to construct a TreeWalkerParameters. The other property should be null.
        /// </summary>
        public ForgeTree ForgeTree { get; set; }

        /// <summary>
        /// The state given to TreeWalker on construction by a wrapper class.
        /// The state holds information that is relevant to TreeWalker while walking the tree.
        /// </summary>
        public IForgeDictionary ForgeState { get; private set; }

        /// <summary>
        /// The ITreeWalkerCallbacks interface defines the callback Tasks that are awaited while walking the tree.
        /// </summary>
        public ITreeWalkerCallbacks Callbacks { get; private set; }

        /// <summary>
        /// The cancellation token.
        /// </summary>
        public CancellationToken Token { get; private set; }

        #endregion

        #region Optional Properties

        /// <summary>
        /// The dynamic object that is able to be referenced when evaluating schema expressions or performing actions.
        /// </summary>
        public object UserContext { get; set; }

        /// <summary>
        /// The Assembly containing ForgeActionAttribute tagged classes.
        /// </summary>
        public Assembly ForgeActionsAssembly { get; set; }

        /// <summary>
        /// Script cache used by ExpressionExecutor to cache and re-use compiled Roslyn scripts.
        /// </summary>
        public ConcurrentDictionary<string, Script<object>> ScriptCache { get; set; }

        /// <summary>
        /// Dependencies required to compile and execute the schema. Null if no external dependencies required.
        /// </summary>
        public List<Type> Dependencies { get; set; }

        /// <summary>
        /// The external executors work similarly to the built-in ExpressionExecutor, but use their own string match and evaluation logic on schema expressions.
        /// The key is the string that EvaluateDynamicProperty will attempt to match string properties against. Similar to "C#|" for Roslyn expressions.
        /// The value is the Func that is called when the string key matches. This Func takes the expression and token, and should return the expected value type.
        /// </summary>
        public Dictionary<string, Func<string, CancellationToken, Task<object>>> ExternalExecutors { get; set; }

        /// <summary>
        /// The delegate called by Forge when executing SubroutineActions to get an initialized TreeWalkerSession that Forge will call WalkTree upon.
        /// The TreeWalkerSession should be set up according to the input to execute the Subroutine session.
        /// Typically this delegate is expected to:
        ///   1. Create new TreeWalkerParameters with passed in TreeName and TreeInput.
        ///   2. Update the JsonSchema that maps from the passed in TreeName.
        ///   3. Create a new ForgeState object. It is important for Subroutine sessions to have their own ForgeState object since the private keyPrefix gets updated.
        ///   4. Use the same RootSessionId.
        ///   5. Other parameters can stay the same or be changed. For example, the App could use a different UserContext or ForgeActionsAssembly for different ForgeTrees if they desire.
        /// </summary>
        public Func<SubroutineInput, Guid, TreeWalkerParameters, TreeWalkerSession> InitializeSubroutineTree { get; set; }

        /// <summary>
        /// The unique identifier of the root/parent session that gets passed on to Subroutine sessions.
        /// RootSessionId will get set to SessionId if not initialized.
        /// </summary>
        public Guid RootSessionId { get; set; }

        /// <summary>
        /// The name of the ForgeTree in the JsonSchema.
        /// For Subroutines, this is evaluated from the SubroutineInput on the schema.
        /// "RootTree" will be used as TreeName if not specified.
        /// </summary>
        public string TreeName { get; set; }

        /// <summary>
        /// The dynamic TreeInput object for this tree walking session.
        /// This is passed in to the root/parent session by the App.
        /// For Subroutines, this is evaluated from the SubroutineInput on the schema.
        /// TreeInput is able to be referenced when evaluating schema expressions.
        /// </summary>
        public object TreeInput { get; set; }

        #endregion

        /// <summary>
        /// Instantiates a TreeWalkerParameters object with the properties that are required to instantiate a TreeWalkerSession object.
        /// </summary>
        /// <param name="sessionId">The unique identifier for this session.</param>
        /// <param name="jsonSchema">The JSON schema.</param>
        /// <param name="forgeState">The Forge state.</param>
        /// <param name="callbacks">The callbacks object.</param>
        /// <param name="token">The cancellation token.</param>
        public TreeWalkerParameters(
            Guid sessionId,
            string jsonSchema,
            IForgeDictionary forgeState,
            ITreeWalkerCallbacks callbacks,
            CancellationToken token)
        {
            if (sessionId == Guid.Empty) throw new ArgumentNullException("sessionId");
            if (string.IsNullOrWhiteSpace(jsonSchema)) throw new ArgumentNullException("jsonSchema");
            if (forgeState == null) throw new ArgumentNullException("forgeState");
            if (callbacks == null) throw new ArgumentNullException("callbacks");
            if (token == null) throw new ArgumentNullException("token");

            this.SessionId = sessionId;
            this.JsonSchema = jsonSchema;
            this.ForgeState = forgeState;
            this.Callbacks = callbacks;
            this.Token = token;
        }

        /// <summary>
        /// Instantiates a TreeWalkerParameters object with the properties that are required to instantiate a TreeWalkerSession object.
        /// </summary>
        /// <param name="sessionId">The unique identifier for this session.</param>
        /// <param name="forgeTree">The ForgeTree for this session.</param>
        /// <param name="forgeState">The Forge state.</param>
        /// <param name="callbacks">The callbacks object.</param>
        /// <param name="token">The cancellation token.</param>
        public TreeWalkerParameters(
            Guid sessionId,
            ForgeTree forgeTree,
            IForgeDictionary forgeState,
            ITreeWalkerCallbacks callbacks,
            CancellationToken token)
        {
            if (sessionId == Guid.Empty) throw new ArgumentNullException("sessionId");
            if (forgeTree == null) throw new ArgumentNullException("forgeTree");
            if (forgeState == null) throw new ArgumentNullException("forgeState");
            if (callbacks == null) throw new ArgumentNullException("callbacks");
            if (token == null) throw new ArgumentNullException("token");

            this.SessionId = sessionId;
            this.ForgeTree = forgeTree;
            this.ForgeState = forgeState;
            this.Callbacks = callbacks;
            this.Token = token;
        }
    }
}