//-----------------------------------------------------------------------
// <copyright file="TestDelayExceptionAction.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The TestDelayExceptionAction class implements the BaseCommonAction abstract class.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.TreeWalker.UnitTests
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Forge.Attributes;
    using Microsoft.Forge.TreeWalker;
    using Newtonsoft.Json;

    [ForgeAction(InputType: typeof(TestDelayExceptionInput))]
    public class TestDelayExceptionAction : BaseCommonAction
    {
        public override async Task<ActionResponse> RunAction()
        {
            TestDelayExceptionInput actionInput = (TestDelayExceptionInput)this.Input;
            Console.WriteLine(string.Format(
                "TestDelayExceptionAction - SessionId: {0}, TreeNodeKey: {1}, ActionInput: {2}.",
                this.SessionId,
                this.TreeNodeKey,
                JsonConvert.SerializeObject(actionInput)));

            await Task.Delay(actionInput.DelayMilliseconds, this.Token);

            if (actionInput.ThrowException)
            {
                throw new NullReferenceException("Throwing unexpected Exception!!");
            }

            ActionResponse actionResponse = new ActionResponse() { Status = "Success" };
            return actionResponse;
        }
    }

    public class TestDelayExceptionInput
    {
        public int DelayMilliseconds { get; set; } = 0;

        public bool ThrowException { get; set; } = false;
    }
}
