//-----------------------------------------------------------------------
// <copyright file="RevisitAction.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The RevisitAction class implements the BaseCommonAction abstract class.
// </summary>
//-----------------------------------------------------------------------

namespace Forge.TreeWalker.UnitTests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Forge.Attributes;
    using Forge.TreeWalker;

    /// <summary>
    /// This action increases counters each time it is visited from the same TreeActionKey in the same SessionId.
    /// It persists these counters in the ActionResponse/PreviousActionResponse.
    /// This tests CommitCurrentTreeNode revisit/cycle behavior.
    /// </summary>
    [ForgeAction]
    public class RevisitAction : BaseCommonAction
    {
        public override async Task<ActionResponse> RunAction()
        {
            // Confirm that intermediates are getting cleared every time we revisit the node, despite us increasing the counter.
            int intermediates = await this.GetIntermediates<int>();
            Assert.AreEqual(0, intermediates);

            intermediates++;
            await this.CommitIntermediates<int>(intermediates);

            // GetPreviousActionResponse, increase the Output counter by one, and return.
            ActionResponse actionResponse = await this.GetPreviousActionResponse() ?? new ActionResponse() { Status = "Success", Output = 0 };
            actionResponse.Output = (int)actionResponse.Output + 1;

            Console.WriteLine(string.Format(
                "RevisitAction - SessionId: {0}, TreeNodeKey: {1}, ActionResponse.Output: {2}.",
                this.SessionId,
                this.TreeNodeKey,
                actionResponse.Output));

            return actionResponse;
        }
    }
}
