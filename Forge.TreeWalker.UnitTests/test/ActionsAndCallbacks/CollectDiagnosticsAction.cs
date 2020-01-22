//-----------------------------------------------------------------------
// <copyright file="CollectDiagnosticsAction.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The CollectDiagnosticsAction class implements the BaseCommonAction abstract class.
// </summary>
//-----------------------------------------------------------------------

namespace Forge.TreeWalker.UnitTests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Forge.Attributes;
    using Forge.TreeWalker;
    using Newtonsoft.Json;

    [ForgeAction(InputType: typeof(CollectDiagnosticsInput))]
    public class CollectDiagnosticsAction : BaseCommonAction
    {
        public override async Task<ActionResponse> RunAction()
        {
            CollectDiagnosticsInput actionInput = (CollectDiagnosticsInput)this.Input;

            // Collect diagnostics using the input command and commit results.
            string intermediates = this.GetIntermediates<string>().GetAwaiter().GetResult();
            Assert.AreEqual(null, intermediates);

            string result = MockCollectDiagnosticsResult(actionInput.Command);

            intermediates = result;
            this.CommitIntermediates<string>(intermediates).GetAwaiter().GetResult();
            Assert.AreEqual(intermediates, this.GetIntermediates<string>().GetAwaiter().GetResult());

            await Task.Run(() => Console.WriteLine(string.Format(
                "CollectDiagnosticsAction - SessionId: {0}, TreeNodeKey: {1}, ActionInput: {2}.",
                this.SessionId,
                this.TreeNodeKey,
                JsonConvert.SerializeObject(actionInput))));

            ActionResponse actionResponse = new ActionResponse() { Status = "Success", Output = result };
            return actionResponse;
        }

        private static string MockCollectDiagnosticsResult(string command)
        {
            return command + "_Results";
        }
    }

    public class CollectDiagnosticsInput
    {
        public string Command { get; set; }
    }
}
