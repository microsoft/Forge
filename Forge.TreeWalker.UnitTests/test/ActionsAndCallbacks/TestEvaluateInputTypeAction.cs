//-----------------------------------------------------------------------
// <copyright file="TestEvaluateInputTypeAction.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The TestEvaluateInputTypeAction class implements the BaseCommonAction abstract class.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.TreeWalker.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Forge.Attributes;
    using Microsoft.Forge.TreeWalker;
    using Newtonsoft.Json;

    [ForgeAction(InputType: typeof(FooActionInput))]
    public class TestEvaluateInputTypeAction : BaseCommonAction
    {
        public override async Task<ActionResponse> RunAction()
        {
            FooActionInput actionInput = (FooActionInput)this.Input;
            bool boolDelegateResult = actionInput.BoolDelegate();
            bool boolDelegateAsyncResult = await actionInput.BoolDelegateAsync();

            Console.WriteLine(string.Format(
                "TestEvaluateInputTypeAction - SessionId: {0}, TreeNodeKey: {1}, ActionInput: {2}, BoolDelegateResult: {3}, BoolDelegateAsyncResult: {4}.",
                this.SessionId,
                this.TreeNodeKey,
                JsonConvert.SerializeObject(actionInput),
                boolDelegateResult,
                boolDelegateAsyncResult));

            ActionResponse actionResponse = new ActionResponse() { Status = "Success", Output = boolDelegateResult && boolDelegateAsyncResult };
            return actionResponse;
        }
    }

    public class FooActionInput
    {
        public string Command { get; set; }
        public int IntExpression { get; set; }
        public bool BoolExpression { get; set; }
        public string PropertyNotInSchema { get; set; }
        public FooActionObject NestedObject { get; set; }
        public FooActionObject[] ObjectArray { get; set; }
        public string[] StringArray { get; set; }
        public long[] LongArray { get; set; }
        public Func<bool> BoolDelegate { get; set; }
        public Func<Task<bool>> BoolDelegateAsync { get; set; }
        public Dictionary<string, string> StringDictionary { get; set; }
        public object DynamicObject { get; set; }
    }

    public class FooActionObject
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int IntPropertyInObject { get; set; }
    }
}
