//-----------------------------------------------------------------------
// <copyright file="TreeWalkerCallbacks.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The TreeWalkerCallbacks class implements the ITreeWalkerCallbacks interface.
// </summary>
//-----------------------------------------------------------------------

namespace Forge.TreeWalker.UnitTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Forge.TreeWalker;
    using Newtonsoft.Json;

    public class TreeWalkerCallbacks : ITreeWalkerCallbacks
    {
        public async Task BeforeVisitNode(
            Guid sessionId,
            string treeNodeKey,
            dynamic properties,
            dynamic userContext,
            string treeName,
            Guid rootSessionId,
            CancellationToken token)
        {
            string serializeProperties = JsonConvert.SerializeObject(properties);

            await Task.Run(() => Console.WriteLine(string.Format(
                "OnBeforeVisitNode: SessionId: {0}, TreeNodeKey: {1}, Properties: {2}.",
                sessionId,
                treeNodeKey,
                serializeProperties)));
        }

        public Task AfterVisitNode(
            Guid sessionId,
            string treeNodeKey,
            dynamic properties,
            dynamic userContext,
            string treeName,
            Guid rootSessionId,
            CancellationToken token)
        {
            Console.WriteLine(string.Format(
                "OnAfterVisitNode: SessionId: {0}, TreeNodeKey: {1}, Properties: {2}.",
                sessionId,
                treeNodeKey,
                JsonConvert.SerializeObject(properties)));

            return Task.FromResult(0);
        }
    }
}
