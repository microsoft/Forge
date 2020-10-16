//-----------------------------------------------------------------------
// <copyright file="ITreeWalkerCallbacksV2.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The ITreeWalkerCallbacksV2 interface. Use TreeNodeContext to wrap all the required input and output info.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.TreeWalker
{
    using System.Threading.Tasks;

    /// <summary>
    /// The ITreeWalkerCallbacks interface defines the callback Tasks that are awaited while walking the tree.
    /// </summary>
    public interface ITreeWalkerCallbacksV2
    {
        /// <summary>
        /// The callback Task that is awaited before visiting each node.
        /// </summary>
        /// <param name="treeNodeContext">The tree node context holding relevant information about this tree node and session.</param>
        Task BeforeVisitNode(TreeNodeContext treeNodeContext);

        /// <summary>
        /// The callback Task that is awaited after visiting each node.
        /// </summary>
        /// <param name="treeNodeContext">The tree node context holding relevant information about this tree node and session.</param>
        Task AfterVisitNode(TreeNodeContext treeNodeContext);
    }
}