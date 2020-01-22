//-----------------------------------------------------------------------
// <copyright file="TestEvaluateInputType_FailOnNonEmptyCtor_Action.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The TestEvaluateInputType_FailOnNonEmptyCtor_Action class implements the BaseCommonAction abstract class.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.TreeWalker.UnitTests
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Forge.Attributes;
    using Microsoft.Forge.TreeWalker;
    using Newtonsoft.Json;

    [ForgeAction(InputType: typeof(FooActionInput_NonEmptyConstructor))]
    public class TestEvaluateInputType_FailOnNonEmptyCtor_Action : BaseCommonAction
    {
        public override async Task<ActionResponse> RunAction()
        {
            FooActionInput_NonEmptyConstructor actionInput = (FooActionInput_NonEmptyConstructor)this.Input;

            await Task.Run(() => Console.WriteLine(string.Format(
                "TestEvaluateInputType_FailOnNonEmptyCtor_Action - SessionId: {0}, TreeNodeKey: {1}, ActionInput: {2}.",
                this.SessionId,
                this.TreeNodeKey,
                JsonConvert.SerializeObject(actionInput))));

            ActionResponse actionResponse = new ActionResponse() { Status = "Success" };
            return actionResponse;
        }
    }

    public class FooActionInput_NonEmptyConstructor
    {
        public bool BoolProperty { get; set; }

        public FooActionInput_NonEmptyConstructor(int unexpectedParameter) { }
    }
}
