//-----------------------------------------------------------------------
// <copyright file="ActionContext.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The ActionContext class.
// </summary>
//-----------------------------------------------------------------------

namespace Forge.TreeWalker
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The ActionContext class object is passed to Actions to give them contextual data and methods to help them execute.
    /// </summary>
    public class ActionContext
    {
        /// <summary>
        /// The unique identifier for this tree walking session.
        /// </summary>
        public Guid SessionId { get; private set; }

        /// <summary>
        /// The tree node key where the Action resides.
        /// </summary>
        public string TreeNodeKey { get; private set; }

        /// <summary>
        /// The tree action key of this Action.
        /// </summary>
        public string TreeActionKey { get; private set; }

        /// <summary>
        /// The name of this Action.
        /// </summary>
        public string ActionName { get; private set; }

        /// <summary>
        /// The dynamic input for the Action given by the ForgeTree schema.
        /// </summary>
        public object ActionInput { get; private set; }

        /// <summary>
        /// The dynamic properties of this Action given by the ForgeTree schema.
        /// </summary>
        public object Properties { get; private set; }

        /// <summary>
        /// The dynamic user-defined context object that is able to be referenced when evaluating schema expressions or performing actions.
        /// </summary>
        public object UserContext { get; private set; }

        /// <summary>
        /// The cancellation token.
        /// </summary>
        public CancellationToken Token { get; private set; }

        /// <summary>
        /// The forgeState dictionary that holds information relevant to Forge and Actions.
        /// </summary>
        private IForgeDictionary forgeState;

        /// <summary>
        /// Instantiates an ActionContext object.
        /// </summary>
        /// <param name="sessionId">The unique identifier for this tree walking session.</param>
        /// <param name="treeNodeKey">The TreeNode's key where the Action is taking place.</param>
        /// <param name="treeActionKey">The TreeAction's key of the Action taking place.</param>
        /// <param name="actionName">The name of the Action.</param>
        /// <param name="actionInput">The input for this Action.</param>
        /// <param name="properties">The properties of this Action.</param>
        /// <param name="userContext">The user context for this Action.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="forgeState">The forge state dictionary.</param>
        public ActionContext(
            Guid sessionId,
            string treeNodeKey,
            string treeActionKey,
            string actionName,
            object actionInput,
            object properties,
            object userContext,
            CancellationToken token,
            IForgeDictionary forgeState)
        {
            if (sessionId == null) throw new ArgumentNullException("sessionId");
            if (string.IsNullOrWhiteSpace(treeNodeKey)) throw new ArgumentNullException("treeNodeKey");
            if (string.IsNullOrWhiteSpace(actionName)) throw new ArgumentNullException("actionName");
            if (userContext == null) throw new ArgumentNullException("userContext");
            if (token == null) throw new ArgumentNullException("token");
            if (forgeState == null) throw new ArgumentNullException("forgeState");

            this.SessionId = sessionId;
            this.TreeNodeKey = treeNodeKey;
            this.TreeActionKey = treeActionKey;
            this.ActionName = actionName;
            this.ActionInput = actionInput;
            this.Properties = properties;
            this.UserContext = userContext;
            this.Token = token;
            this.forgeState = forgeState;
        }

        /// <summary>
        /// Commits an Intermediates object for this Action to the forgeState.
        /// Since Intermediates are available to the Action on each retry, this allows Actions to persist state across retries.
        /// </summary>
        /// <param name="intermediates">The intermediates object to be committed for this Action.</param>
        public Task CommitIntermediates<T>(T intermediates)
        {
            return this.forgeState.Set<T>(this.TreeActionKey + TreeWalkerSession.IntermediatesSuffix, intermediates);
        }

        /// <summary>
        /// Gets the previously committed Intermediates data for this Action from the forgeState.
        /// </summary>
        /// <returns>The Intermediates data for this Action if it exists, otherwise default(T).</returns>
        public async Task<T> GetIntermediates<T>()
        {
            try
            {
                return await this.forgeState.GetValue<T>(this.TreeActionKey + TreeWalkerSession.IntermediatesSuffix).ConfigureAwait(false);
            }
            catch
            {
                return default(T);
            }
        }
    }
}