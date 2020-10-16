//-----------------------------------------------------------------------
// <copyright file="ITreeWalkerCallbacks.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The ITreeWalkerCallbacks interface.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.TreeWalker
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// [DEPRECATED]
    /// The ITreeWalkerCallbacks interface defines the callback Tasks that are awaited while walking the tree.
    /// Prefers future callers to use the new ITreeWalkerCallbacksV2.
    /// </summary>
    public interface ITreeWalkerCallbacks
    {
        /// <summary>
        /// The callback Task that is awaited before visiting each node.
        /// </summary>
        /// <param name="sessionId">The Id of this tree walking session.</param>
        /// <param name="treeNodeKey">The key of the current tree node being visited by Forge.</param>
        /// <param name="properties">The additional properties for this node.</param>
        /// <param name="userContext">The dynamic user-defined context object.</param>
        /// <param name="treeName">The name of the ForgeTree in the JsonSchema.</param>
        /// <param name="rootSessionId">The unique identifier for the root/parent tree walking session.</param>
        /// <param name="token">The cancellation token.</param>
        Task BeforeVisitNode(Guid sessionId, string treeNodeKey, dynamic properties, object userContext, string treeName, Guid rootSessionId, CancellationToken token);

        /// <summary>
        /// The callback Task that is awaited after visiting each node.
        /// </summary>
        /// <param name="sessionId">The Id of this tree walking session.</param>
        /// <param name="treeNodeKey">The key of the current tree node being visited by Forge.</param>
        /// <param name="properties">The additional properties for this node.</param>
        /// <param name="userContext">The dynamic user-defined context object.</param>
        /// <param name="treeName">The name of the ForgeTree in the JsonSchema.</param>
        /// <param name="rootSessionId">The unique identifier for the root/parent tree walking session.</param>
        /// <param name="token">The cancellation token.</param>
        Task AfterVisitNode(Guid sessionId, string treeNodeKey, dynamic properties, object userContext, string treeName, Guid rootSessionId, CancellationToken token);
    }
}