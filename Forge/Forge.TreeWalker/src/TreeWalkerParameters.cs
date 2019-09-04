//-----------------------------------------------------------------------
// <copyright file="TreeWalkerParameters.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The TreeWalkerParameters class.
// </summary>
//-----------------------------------------------------------------------

namespace Forge.TreeWalker
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Scripting;

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
        /// </summary>
        public string JsonSchema { get; private set; }

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

        #endregion

        /// <summary>
        /// Instantiates a TreeWalkerParameters object with the properies that are required to instantiate a TreeWalkerSession object.
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
    }
}