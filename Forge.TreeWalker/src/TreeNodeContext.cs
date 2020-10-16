//-----------------------------------------------------------------------
// <copyright file="TreeNodeContext.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The TreeNodeContext class.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.TreeWalker
{
    using System;
    using System.Threading;

    /// <summary>
    /// The TreeNodeContext class object is passed to Callbacks.BeforeVisitNode and Callbacks.AfterVisitNode.
    /// </summary>
    public class TreeNodeContext
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
        /// The dynamic properties of this Node given by the ForgeTree schema.
        /// </summary>
        public dynamic Properties { get; private set; }

        /// <summary>
        /// The dynamic user-defined context object that is able to be referenced when evaluating schema expressions or performing actions.
        /// </summary>
        public object UserContext { get; private set; }

        /// <summary>
        /// The cancellation token.
        /// </summary>
        public CancellationToken Token { get; private set; }

        /// <summary>
        /// The name of the ForgeTree in the JsonSchema.
        /// </summary>
        public string TreeName { get; private set; }

        /// <summary>
        /// The unique identifier for the root/parent tree walking session.
        /// </summary>
        public Guid RootSessionId { get; private set; }

        /// <summary>
        /// When set, the tree walker should skip all actions defined in this node, and proceed with ChildSelector.
        /// When not null or whitespace, the tree walker will skip all actions defined in the current tree node, and proceed to AfterVisitNode then ChildSelector.
        /// Update this property inside BeforeVisitNode if you wish to use this feature for the current tree node.
        /// The string context is available to check in the current TreeNode's ChildSelector via Session.GetCurrentNodeSkipActionContext().
        /// </summary>
        public string CurrentNodeSkipActionContext { get; set; }

        /// <summary>
        /// Instantiates an TreeNodeContext object.
        /// </summary>
        /// <param name="sessionId">The unique identifier for this tree walking session.</param>
        /// <param name="treeNodeKey">The tree node's key.</param>
        /// <param name="properties">The properties of this tree node.</param>
        /// <param name="userContext">Optional, the user context for this tree node.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="treeName">The name of the ForgeTree in the JsonSchema.</param>
        /// <param name="rootSessionId">The unique identifier for the root/parent tree walking session.</param>
        /// <param name="currentNodeSkipActionContext">
        /// The string context if the actions in this tree node are skipped. It is null when no skip. When it is not empty, proceed with ChildSelector.
        /// </param>
        public TreeNodeContext(
            Guid sessionId,
            string treeNodeKey,
            dynamic properties,
            object userContext,
            CancellationToken token,
            string treeName,
            Guid rootSessionId,
            string currentNodeSkipActionContext)
        {
            if (sessionId == null) throw new ArgumentNullException("sessionId");
            if (string.IsNullOrWhiteSpace(treeNodeKey)) throw new ArgumentNullException("treeNodeKey");
            if (token == null) throw new ArgumentNullException("token");
            if (string.IsNullOrWhiteSpace(treeName)) throw new ArgumentNullException("treeName");
            if (rootSessionId == null) throw new ArgumentNullException("rootSessionId");

            this.SessionId = sessionId;
            this.TreeNodeKey = treeNodeKey;
            this.Properties = properties;
            this.UserContext = userContext;
            this.Token = token;
            this.TreeName = treeName;
            this.RootSessionId = rootSessionId;
            this.CurrentNodeSkipActionContext = currentNodeSkipActionContext;
        }
    }
}