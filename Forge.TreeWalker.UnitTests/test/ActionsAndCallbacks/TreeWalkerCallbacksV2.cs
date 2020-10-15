//-----------------------------------------------------------------------
// <copyright file="TreeWalkerCallbacksV2.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The TreeWalkerCallbacksV2 class implements the ITreeWalkerCallbacksV2 interface.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.TreeWalker.UnitTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Forge.TreeWalker;
    using Newtonsoft.Json;

    public class TreeWalkerCallbacksV2 : ITreeWalkerCallbacksV2
    {
        /// <summary>
        /// Get the string context if the actions in this TreeNodes are skipped.
        /// Usually, when there is no action skipped, the return value is null.
        /// </summary>
        public string CurrentNodeSkipActionContext { get; set; }

        public async Task BeforeVisitNode(TreeNodeContext treeNodeContext)
        {
            string serializeProperties = JsonConvert.SerializeObject(treeNodeContext.Properties);

            await Task.Run(() => Console.WriteLine(string.Format(
                "OnBeforeVisitNode: SessionId: {0}, TreeNodeKey: {1}, Properties: {2}.",
                treeNodeContext.SessionId,
                treeNodeContext.TreeNodeKey,
                serializeProperties)));

            treeNodeContext.CurrentNodeSkipActionContext = this.CurrentNodeSkipActionContext;
        }

        public async Task AfterVisitNode(TreeNodeContext treeNodeContext)
        {
            string serializeProperties = JsonConvert.SerializeObject(treeNodeContext.Properties);

            await Task.Run(() => Console.WriteLine(string.Format(
                "AfterVisitNode: SessionId: {0}, TreeNodeKey: {1}, Properties: {2}.",
                treeNodeContext.SessionId,
                treeNodeContext.TreeNodeKey,
                serializeProperties)));

            treeNodeContext.CurrentNodeSkipActionContext = this.CurrentNodeSkipActionContext;
        }
    }
}
