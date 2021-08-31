//-----------------------------------------------------------------------
// <copyright file="ReturnSessionIdAction.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The ReturnSessionIdAction class implements the BaseCommonAction abstract class.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.TreeWalker.UnitTests
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Forge.Attributes;
    using Microsoft.Forge.TreeWalker;

    [ForgeAction]
    public class GetCurrentTimeStamp : BaseCommonAction
    {
        public override Task<ActionResponse> RunAction()
        {
            return Task.FromResult(new ActionResponse() { Status = DateTime.UtcNow.ToString(), StatusCode = 1 });
        }
    }
}
