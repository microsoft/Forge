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
    /// The TreeNodeContext class holds relevant information about the tree node and session.
    /// This object gets passed to CallbacksV2.BeforeVisitNode and CallbacksV2.AfterVisitNode.
    /// </summary>
    public class TreeNodeContext
    {
        /// <summary>
        /// The Id of this tree walking session.
        /// </summary>
        public Guid SessionId { get; private set; }

        /// <summary>
        /// The key of the current tree node being visited by Forge.
        /// </summary>
        public string TreeNodeKey { get; private set; }

        /// <summary>
        /// The additional properties for this node.
        /// </summary>
        public dynamic Properties { get; private set; }

        /// <summary>
        /// The dynamic user-defined context object.
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
        /// When set, the tree walker will skip all actions defined in the current tree node, and proceed to AfterVisitNode then ChildSelector.
        /// Update this property inside BeforeVisitNode if you wish to use this feature for the current tree node.
        /// The string context is available to check in the current TreeNode's ChildSelector via Session.GetCurrentNodeSkipActionContext().
        /// </summary>
        public string CurrentNodeSkipActionContext { get; set; }

        /// <summary>
        /// Instantiates an TreeNodeContext object.
        /// </summary>
        /// <param name="sessionId">The Id of this tree walking session.</param>
        /// <param name="treeNodeKey">The key of the current tree node being visited by Forge.</param>
        /// <param name="properties">The additional properties for this node.</param>
        /// <param name="userContext">The dynamic user-defined context object.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="treeName">The name of the ForgeTree in the JsonSchema.</param>
        /// <param name="rootSessionId">The unique identifier for the root/parent tree walking session.</param>
        /// <param name="currentNodeSkipActionContext">
        /// The string context if the actions in the current tree node should be skipped, or null if actions should not be skipped.
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