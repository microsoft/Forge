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
    using System.Threading.Tasks;

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
        /// Indicates whether the tree walker should skip all actions defined in this node, and proceed with ChildSelector.
        /// </summary>
        /// <remarks>Allow Public set for testing purpose.</remarks>
        public bool ShouldSkipActionsInTreeNode { get; set; }

        /// <summary>
        /// Instantiates an TreeNodeContext object.
        /// </summary>
        /// <param name="sessionId">The unique identifier for this tree walking session.</param>
        /// <param name="treeNodeKey">The TreeNode's key where the Action is taking place.</param>
        /// <param name="properties">The properties of this Tree node..</param>
        /// <param name="userContext">The user context for this Action.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="treeName">The name of the ForgeTree in the JsonSchema.</param>
        /// <param name="rootSessionId">The unique identifier for the root/parent tree walking session.</param>
        /// <param name="ShouldSkipActionsInTreeNode">Indicates whether the tree walker should skip all actions defined in this node, and proceed with ChildSelector</param>
        public TreeNodeContext(
            Guid sessionId,
            string treeNodeKey,
            dynamic properties,
            object userContext,
            CancellationToken token,
            string treeName,
            Guid rootSessionId,
            bool shouldSkipActionsInTreeNode)
        {
            if (sessionId == null) throw new ArgumentNullException("sessionId");
            if (string.IsNullOrWhiteSpace(treeNodeKey)) throw new ArgumentNullException("treeNodeKey");
            if (userContext == null) throw new ArgumentNullException("userContext");
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
            this.ShouldSkipActionsInTreeNode = shouldSkipActionsInTreeNode;
        }
    }
}