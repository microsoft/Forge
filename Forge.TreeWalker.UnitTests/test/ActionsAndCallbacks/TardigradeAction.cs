//-----------------------------------------------------------------------
// <copyright file="TardigradeAction.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The TardigradeAction class implements the BaseCommonAction abstract class.
// </summary>
//-----------------------------------------------------------------------

namespace Forge.TreeWalker.UnitTests
{
    using System;
    using System.Threading.Tasks;

    using Forge.Attributes;
    using Forge.TreeWalker;

    [ForgeAction]
    public class TardigradeAction : BaseCommonAction
    {
        public override async Task<ActionResponse> RunAction()
        {
            await Task.Run(() => Console.WriteLine(string.Format(
                "TardigradeAction - SessionId: {0}, TreeNodeKey: {1}.",
                this.SessionId,
                this.TreeNodeKey)));

            ActionResponse actionResponse = new ActionResponse() { Status = "Success" };
            return actionResponse;
        }
    }
}
