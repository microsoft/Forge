//-----------------------------------------------------------------------
// <copyright file="TreeWalkerUnitTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Tests the TreeWalkerSession class.
// </summary>
//-----------------------------------------------------------------------

namespace Forge.TreeWalker.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Forge.Attributes;
    using Forge.DataContracts;
    using Forge.TreeWalker;
    using Forge.TreeWalker.ForgeExceptions;

    using Newtonsoft.Json;

    [TestClass]
    public class TreeWalkerUnitTests
    {
        private Guid sessionId;
        private IForgeDictionary forgeState = new ForgeDictionary(new Dictionary<string, object>(), Guid.Empty);
        private dynamic UserContext = new System.Dynamic.ExpandoObject();
        private ITreeWalkerCallbacks callbacks;
        private CancellationToken token;
        private TreeWalkerParameters parameters;
        private TreeWalkerSession session;

        public void TestInitialize(string jsonSchema)
        {
            // Initialize contexts, callbacks, and actions.
            this.sessionId = Guid.NewGuid();
            this.forgeState = new ForgeDictionary(new Dictionary<string, object>(), this.sessionId);
            this.callbacks = new TreeWalkerCallbacks();
            this.token = new CancellationTokenSource().Token;

            this.UserContext.Name = "MyName";
            this.UserContext.ResourceType = "Container";

            this.UserContext.GetCount = (Func<Int32>)(() =>
            {
                return 1;
            });

            this.UserContext.GetCountAsync = (Func<Task<Int32>>)(() =>
            {
                return Task.FromResult(2);
            });

            this.parameters = new TreeWalkerParameters(
                this.sessionId,
                jsonSchema,
                this.forgeState,
                this.callbacks,
                this.token)
            {
                UserContext = this.UserContext,
                ForgeActionsAssembly = typeof(CollectDiagnosticsAction).Assembly
            };

            this.session = new TreeWalkerSession(this.parameters);
        }

        [TestMethod]
        public void TestTreeWalkerSession_Constructor()
        {
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.TardigradeScenario);

            // Test 1 - Verify jsonSchema was successfully deserialized in constructor.
            Assert.AreEqual("Action", this.session.Schema.Tree["Tardigrade"].Type.ToString());

            // Test 2 - Verify the Status is Initialized.
            Assert.AreEqual("Initialized", this.session.Status, "Expected WalkTree status to be Initialized after initializing TreeWalkerSession.");
        }

        [TestMethod]
        public void TestTreeWalkerSession_VisitNode_Success()
        {
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.TardigradeScenario);

            // Test - VisitNode and expect the first child to be returned.
            string expected = "Tardigrade";
            string actualNextTreeNodeKey = this.session.VisitNode("Container").GetAwaiter().GetResult();

            Assert.AreEqual(expected, actualNextTreeNodeKey, "Expected VisitNode(Container) to return Tardigrade.");
        }

        [TestMethod]
        public void TestTreeWalkerSession_VisitNode_LeafNode_Success()
        {
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.TardigradeScenario);

            // Test - VisitNode on node of Leaf type and confirm it does not throw.
            string expected = null;
            string actualNextTreeNodeKey = this.session.VisitNode("Tardigrade_Success").GetAwaiter().GetResult();

            Assert.AreEqual(expected, actualNextTreeNodeKey, "Expected VisitNode(Tardigrade_Success) to return without throwing.");
        }

        [TestMethod]
        public void TestTreeWalkerSession_VisitNode_NoTimeout_Success()
        {
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.TardigradeScenario);

            // Test - VisitNode with no Timeout and execute an Action with no Timeout set. Confirm we do not throw exceptions.
            string expected = "Tardigrade_Success";
            string actualNextTreeNodeKey = this.session.VisitNode("Tardigrade").GetAwaiter().GetResult();
            Assert.AreEqual(expected, actualNextTreeNodeKey, "Expected VisitNode(Tardigrade) to return Tardigrade_Success without throwing exception.");
        }

        [TestMethod]
        public void TestTreeWalkerSession_WalkTree_Success()
        {
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.TardigradeScenario);

            // Test - WalkTree and expect the Status to be RanToCompletion.
            string actualStatus = this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("RanToCompletion", actualStatus, "Expected WalkTree to run to completion.");
        }

        [TestMethod]
        public void TestTreeWalkerSession_WalkTree_ActionThrowsException_TimeoutOnAction()
        {
            // Initialize TreeWalkerSession with a schema containing Action that throws exception.
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.ActionException_Fail);

            // Test - WalkTree and expect the Status to be TimeoutOnAction due to unexpected exceptions thrown in action.
            Assert.ThrowsException<ActionTimeoutException>(() => 
            {
                string temp = this.session.WalkTree("Root").GetAwaiter().GetResult();
            }, "Expected WalkTree to timeout on action because the Action threw exceptions with no Continuation flags set.");

            string actualStatus = this.session.Status;
            Assert.AreEqual("TimeoutOnAction", actualStatus, "Expected WalkTree to timeout on action because the Action threw exceptions with no Continuation flags set.");
        }

        [TestMethod]
        public void TestTreeWalkerSession_WalkTree_ActionThrowsException_ContinuationOnRetryExhaustion()
        {
            // Initialize TreeWalkerSession with a schema containing Action that throws exception but has ContinuationOnRetryExhaustion flag set.
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.ActionException_ContinuationOnRetryExhaustion);

            // Test - Expect WalkTree to be successful because the TreeAction exhausted retries but ContinuationOnRetryExhaustion flag was set.
            string actualStatus = this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("RanToCompletion", actualStatus, "Expected WalkTree to be successful because the TreeAction exhausted retries but ContinuationOnRetryExhaustion flag was set.");

            ActionResponse actionResponse = this.session.GetLastActionResponse();
            Assert.AreEqual("RetryExhaustedOnAction", actionResponse.Status, "Expected WalkTree to be successful because the TreeAction exhausted retries but ContinuationOnRetryExhaustion flag was set.");
        }

        [TestMethod]
        public void TestTreeWalkerSession_WalkTree_ActionHasDelay_TimeoutOnAction()
        {
            // Initialize TreeWalkerSession with a schema containing Action that will time out.
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.ActionDelay_Fail);

            // Test - WalkTree and expect the Status to be TimeoutOnAction due to Action timing out.
            Assert.ThrowsException<ActionTimeoutException>(() => 
            {
                string temp = this.session.WalkTree("Root").GetAwaiter().GetResult();
            }, "Expected WalkTree to timeout on action because the Action timed out with no Continuation flags set.");

            string actualStatus = this.session.Status;
            Assert.AreEqual("TimeoutOnAction", actualStatus, "Expected WalkTree to timeout on action because the Action timed out with no Continuation flags set.");
        }

        [TestMethod]
        public void TestTreeWalkerSession_WalkTree_ActionHasDelay_ContinuationOnTimeout()
        {
            // Initialize TreeWalkerSession with a schema containing Action that will time out but has ContinuationOnTimeout flag set.
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.ActionDelay_ContinuationOnTimeout);

            // Test - Expect WalkTree to be successful because the TreeAction timed out but ContinuationOnTimeout flag was set.
            string actualStatus = this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("RanToCompletion", actualStatus, "Expected WalkTree to be successful because the TreeAction timed out but ContinuationOnTimeout flag was set.");

            ActionResponse actionResponse = this.session.GetLastActionResponse();
            Assert.AreEqual("TimeoutOnAction", actionResponse.Status, "Expected WalkTree to be successful because the TreeAction timed out but ContinuationOnTimeout flag was set.");
        }

        [TestMethod]
        public void TestTreeWalkerSession_WalkTree_ActionHasDelay_ContinuationOnTimeout_RetryPolicy_TimeoutInAction()
        {
            // Initialize TreeWalkerSession with a schema containing Action with RetryPolicy that will time out inside the Action but has ContinuationOnTimeout flag set.
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.ActionDelay_ContinuationOnTimeout_RetryPolicy_TimeoutInAction);

            // Test - Expect WalkTree to be successful because the TreeAction timed out but ContinuationOnTimeout flag was set.
            string actualStatus = this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("RanToCompletion", actualStatus, "Expected WalkTree to be successful because the TreeAction timed out but ContinuationOnTimeout flag was set.");

            ActionResponse actionResponse = this.session.GetLastActionResponse();
            Assert.AreEqual("TimeoutOnAction", actionResponse.Status, "Expected WalkTree to be successful because the TreeAction timed out but ContinuationOnTimeout flag was set.");
        }

        [TestMethod]
        public void TestTreeWalkerSession_WalkTree_ActionHasDelay_ContinuationOnTimeout_RetryPolicy_TimeoutBetweenRetries()
        {
            // Initialize TreeWalkerSession with a schema containing Action with RetryPolicy that will time out between retry attempts but has ContinuationOnTimeout flag set.
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.ActionDelay_ContinuationOnTimeout_RetryPolicy_TimeoutBetweenRetries);

            // Test - Expect WalkTree to be successful because the TreeAction timed out but ContinuationOnTimeout flag was set.
            string actualStatus = this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("RanToCompletion", actualStatus, "Expected WalkTree to be successful because the TreeAction timed out but ContinuationOnTimeout flag was set.");

            ActionResponse actionResponse = this.session.GetLastActionResponse();
            Assert.AreEqual("TimeoutOnAction", actionResponse.Status, "Expected WalkTree to be successful because the TreeAction timed out but ContinuationOnTimeout flag was set.");
        }

        [TestMethod]
        public void TestTreeWalkerSession_WalkTree_CancelledBeforeExecution()
        {
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.TardigradeScenario);

            // Test - CancelWalkTree before WalkTree and expect the Status to be CancelledBeforeExecution.
            this.session.CancelWalkTree();

            Assert.ThrowsException<TaskCanceledException>(() => 
            {
                string temp = this.session.WalkTree("Root").GetAwaiter().GetResult();
            }, "Expected WalkTree to throw exception after calling CancelWalkTree.");

            string actualStatus = this.session.Status;
            Assert.AreEqual("CancelledBeforeExecution", actualStatus, "Expected WalkTree to be cancelled before execution after calling CancelWalkTree.");
        }

        [TestMethod]
        public void TestTreeWalkerSession_WalkTree_ActionWithDelay_CancelWalkTree()
        {
            // Initialize TreeWalkerSession with a schema containing Action with delay.
            // This gives us time to start WalkTree before calling CancelWalkTree.
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.ActionDelay_ContinuationOnTimeout_RetryPolicy_TimeoutInAction);

            // Test - WalkTree then CancelWalkTree while WalkTree is running and expect the Status to be Cancelled.
            Task<string> task = this.session.WalkTree("Root");
            Thread.Sleep(25);
            this.session.CancelWalkTree();
            Assert.ThrowsException<OperationCanceledException>(() => 
            {
                string temp = task.GetAwaiter().GetResult();
            }, "Expected WalkTree to throw exception after calling CancelWalkTree.");

            string actualStatus = this.session.Status;
            Assert.AreEqual("Cancelled", actualStatus, "Expected WalkTree to be cancelled after calling CancelWalkTree.");
        }

        [TestMethod]
        public void TestTreeWalkerSession_WalkTree_Failed_MissingKey()
        {
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.TardigradeScenario);

            // Test - WalkTree and expect the Status to be Failed because the key does not exist which threw an exception.
            Assert.ThrowsException<KeyNotFoundException>(() => 
            {
                string temp = this.session.WalkTree("MissingKey").GetAwaiter().GetResult();
            }, "Expected WalkTree to fail because the key does not exist.");

            string actualStatus = this.session.Status;
            Assert.AreEqual("Failed", actualStatus, "Expected WalkTree to fail because the key does not exist.");
        }

        [TestMethod]
        public void TestTreeWalkerSession_WalkTree_NoChildMatched()
        {
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.NoChildMatch);

            // Test - WalkTree and expect the Status to be NoChildMatched.
            string actualStatus = this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("RanToCompletion_NoChildMatched", actualStatus, "Expected WalkTree to end with NoChildMatched status.");
        }

        [TestMethod]
        public void TestGetCurrentTreeNode()
        {
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.TardigradeScenario);

            // Test 1 - Confirm GetCurrentTreeNode returns null before walking tree.
            Assert.AreEqual(null, this.session.GetCurrentTreeNode().GetAwaiter().GetResult(), "Expected CurrentTreeNode to be null before starting walk tree.");

            // Test 2 - Confirm GetCurrentTreeNode returns last node visited after walking tree.
            this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("Tardigrade_Success", this.session.GetCurrentTreeNode().GetAwaiter().GetResult(), "Expected CurrentTreeNode to equal the last node visited.");
        }

        [TestMethod]
        public void TestGetLastTreeAction()
        {
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.TardigradeScenario);

            // Test 1 - Confirm GetLastTreeAction returns null before walking tree.
            Assert.AreEqual(null, this.session.GetLastTreeAction().GetAwaiter().GetResult(), "Expected LastTreeAction to be null before starting walk tree.");

            // Test 2 - Confirm GetLastTreeAction returns last tree action executed after walking tree.
            this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("Tardigrade_TardigradeAction", this.session.GetLastTreeAction().GetAwaiter().GetResult(), "Expected LastTreeAction to equal the last tree action executed.");
        }

        [TestMethod]
        public void TestGetOutput()
        {
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.TardigradeScenario);

            string actualStatus = this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("RanToCompletion", actualStatus, "Expected WalkTree to run to completion.");

            // Test 1 - Confirm ActionResponse can be read from GetOutputAsync.
            ActionResponse actionResponse = this.session.GetOutputAsync("Container_CollectDiagnosticsAction").GetAwaiter().GetResult();
            Assert.AreEqual(
                "RunCollectDiagnostics.exe_Results",
                actionResponse.Output,
                "Expected to successfully read ActionResponse.Output.");

            Assert.AreEqual(
                "Success",
                actionResponse.Status,
                "Expected to successfully read ActionResponse.Status.");

            // Test 2 - Confirm ActionResponse can be read from GetOutput.
            actionResponse = this.session.GetOutput("Container_CollectDiagnosticsAction");
            Assert.AreEqual(
                "RunCollectDiagnostics.exe_Results",
                actionResponse.Output,
                "Expected to successfully read ActionResponse.Output.");

            Assert.AreEqual(
                "Success",
                actionResponse.Status,
                "Expected to successfully read ActionResponse.Status.");

            // Test 3 - Confirm ActionResponse can be read from GetLastActionResponseAsync.
            actionResponse = this.session.GetLastActionResponseAsync().GetAwaiter().GetResult();
            Assert.AreEqual(
                "Success",
                actionResponse.Status,
                "Expected to successfully read ActionResponse.Status.");

            // Test 4 - Confirm ActionResponse can be read from GetLastActionResponse.
            actionResponse = this.session.GetLastActionResponse();
            Assert.AreEqual(
                "Success",
                actionResponse.Status,
                "Expected to successfully read ActionResponse.Status.");
        }

        // TODO: Add back test once we decide how we want to handle this case: Should people be able to change function definitions?
        // TestScript in  local jsonSchema is related to this test.
        // [TestMethod]
        // public void TestTreeWalkerSession_WalkTree_Success_ChangingFunctionDefinition()
        // {
        //     this.TestInitialize(jsonSchema: ForgeSchemaHelper.TardigradeScenario);

        //     Assert.AreEqual(this.UserContext.GetCount(), 1);
        //     string actual = this.session.WalkTree("TestScript").GetAwaiter().GetResult();   
        //     Assert.AreEqual(2, this.UserContext.GetCount());
        // }

        // TODO: Add back test once we decide how we want to handle this case: Should people be able to change function definitions?
        // TestScript in  local jsonSchema is related to this test.

        [TestMethod]
        public void Test_EvaluateInputType_Success()
        {
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.TestEvaluateInputTypeAction);

            // Test - WalkTree to execute an Action with its ActionInput type defined in the ActionDefinition.InputType.
            string actualStatus = this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("RanToCompletion", actualStatus);

            Assert.AreEqual(
                true,
                this.session.GetLastActionResponse().Output,
                "Expected to successfully retrieve the Func output value from the action.");
        }

        [TestMethod]
        public void Test_EvaluateInputType_UnexpectedFieldFail()
        {
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.TestEvaluateInputType_FailOnField_Action);

            // Test - WalkTree and expect the Status to be Failed_EvaluateDynamicProperty
            //        because ActionInput type defined in the ActionDefinition.InputType contained an unexpected public Field.
            string actual;
            Assert.ThrowsException<EvaluateDynamicPropertyException>(() => 
            {
                actual = this.session.WalkTree("Root").GetAwaiter().GetResult();
            }, "Expected WalkTree to fail because ActionInput type defined in the ActionDefinition.InputType contained an unexpected public Field.");

            actual = this.session.Status;
            Assert.AreEqual(
                "Failed_EvaluateDynamicProperty",
                actual,
                "Expected WalkTree to fail because ActionInput type defined in the ActionDefinition.InputType contained an unexpected public Field.");
        }

        [TestMethod]
        public void Test_EvaluateInputType_UnexpectedPropertyFail()
        {
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.TestEvaluateInputTypeAction_UnexpectedPropertyFail);

            // Test - WalkTree and expect the Status to be Failed_EvaluateDynamicProperty
            //        because the schema contained a Property that does not exist in ActionDefinition.InputType.
            string actual;
            Assert.ThrowsException<EvaluateDynamicPropertyException>(() => 
            {
                actual = this.session.WalkTree("Root").GetAwaiter().GetResult();
            }, "Expected WalkTree to fail because the schema contained a Property that does not exist in ActionDefinition.InputType.");

            actual = this.session.Status;
            Assert.AreEqual(
                "Failed_EvaluateDynamicProperty",
                actual,
                "Expected WalkTree to fail because the schema contained a Property that does not exist in ActionDefinition.InputType.");
        }

        [TestMethod]
        public void Test_EvaluateInputType_ParameterizedConstructorFail()
        {
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.TestEvaluateInputType_FailOnNonEmptyCtor_Action);

            // Test - WalkTree and expect the Status to be Failed_EvaluateDynamicProperty
            //        because its ActionDefinition.InputType did not have a parameterless constructor.
            string actual;
            Assert.ThrowsException<EvaluateDynamicPropertyException>(() => 
            {
                actual = this.session.WalkTree("Root").GetAwaiter().GetResult();
            }, "Expected WalkTree to fail because its ActionDefinition.InputType did not have a parameterless constructor.");

            actual = this.session.Status;
            Assert.AreEqual(
                "Failed_EvaluateDynamicProperty",
                actual,
                "Expected WalkTree to fail because its ActionDefinition.InputType did not have a parameterless constructor.");
        }

        [TestMethod]
        public void Test_LeafNodeSummaryAction_Success()
        {
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.LeafNodeSummaryAction);

            // Test - WalkTree to execute a LeafNodeSummaryAction node with its ActionInput set to ActionResponse properties.
            string actualStatus = this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("RanToCompletion", actualStatus);

            ActionResponse leafActionResponse = this.session.GetLastActionResponse();
            Assert.AreEqual(
                "Success",
                leafActionResponse.Status,
                "Expected to successfully retrieve the Func output value from the action.");

            Assert.AreEqual(
                1,
                leafActionResponse.StatusCode,
                "Expected to successfully retrieve the Func output value from the action.");

            Assert.AreEqual(
                "TheResult",
                leafActionResponse.Output,
                "Expected to successfully retrieve the Func output value from the action.");
        }

        [TestMethod]
        public void Test_LeafNodeSummaryAction_InputAsObject_Success()
        {
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.LeafNodeSummaryAction_InputIsActionResponse);

            // Test - WalkTree to execute a LeafNodeSummaryAction node with its ActionInput set to ActionResponse object of the previously ran Action in the parent node.
            string actualStatus = this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("RanToCompletion", actualStatus);

            ActionResponse leafActionResponse = this.session.GetLastActionResponse();
            Assert.AreEqual(
                "Success",
                leafActionResponse.Status,
                "Expected to successfully retrieve the Func output value from the action.");

            Assert.AreEqual(
                "TheCommand_Results",
                leafActionResponse.Output,
                "Expected to successfully retrieve the Func output value from the action.");
        }

        [TestMethod]
        public void Test_ExternalExecutors()
        {
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.ExternalExecutors);

            Dictionary<string, Func<string, CancellationToken, Task<object>>> externalExecutors = new Dictionary<string, Func<string, CancellationToken, Task<object>>>();
	        externalExecutors.Add("External|", External);

            this.parameters.ExternalExecutors = externalExecutors;
            this.session = new TreeWalkerSession(this.parameters);

            // Test - WalkTree to execute an Action with an ActionInput that uses an external executor. Confirm expected results.
            string actualStatus = this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("RanToCompletion", actualStatus);

            ActionResponse leafActionResponse = this.session.GetLastActionResponse();
            Assert.AreEqual(
                "StatusResult_Executed",
                leafActionResponse.Status,
                "Expected to successfully retrieve the Func output value from the action.");
        }

        private static async Task<object> External(string expression, CancellationToken token)
        {
            // External executes the expression and returns.
            await Task.Delay(1, token);
            return expression + "_Executed";
        }

        private sealed class TreeWalkerCallbacks : ITreeWalkerCallbacks
        {
            public async Task BeforeVisitNode(
                Guid sessionId,
                string treeNodeKey,
                dynamic properties,
                dynamic userContext,
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

        public abstract class BaseCommonAction : BaseAction
        {
            public object Input { get; private set; }

            public CancellationToken Token { get; private set; }

            public Guid SessionId { get; private set; }

            public string TreeNodeKey { get; private set; }

            private ActionContext actionContext;

            public override Task<ActionResponse> RunAction(ActionContext actionContext)
            {
                this.Input = actionContext.ActionInput;
                this.Token = actionContext.Token;
                this.SessionId = actionContext.SessionId;
                this.TreeNodeKey = actionContext.TreeNodeKey;
                this.actionContext = actionContext;

                return this.RunAction();
            }

            public abstract Task<ActionResponse> RunAction();

            public Task CommitIntermediates<T>(T intermediates)
            {
                return this.actionContext.CommitIntermediates<T>(intermediates);
            }

            public Task<T> GetIntermediates<T>()
            {
                return this.actionContext.GetIntermediates<T>();
            }
        }

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
        }

        public class FooActionObject
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        [ForgeAction(InputType: typeof(FooActionInput_UnexpectedField))]
        public class TestEvaluateInputType_FailOnField_Action : BaseCommonAction
        {
            public override async Task<ActionResponse> RunAction()
            {
                FooActionInput_UnexpectedField actionInput = (FooActionInput_UnexpectedField)this.Input;

                await Task.Run(() => Console.WriteLine(string.Format(
                    "TestEvaluateInputType_FailOnField_Action - SessionId: {0}, TreeNodeKey: {1}, ActionInput: {2}.",
                    this.SessionId,
                    this.TreeNodeKey,
                    JsonConvert.SerializeObject(actionInput))));

                ActionResponse actionResponse = new ActionResponse() { Status = "Success" };
                return actionResponse;
            }
        }

        public class FooActionInput_UnexpectedField
        {
            public bool UnexpectedField = false;
        }

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

            public FooActionInput_NonEmptyConstructor(int unexpectedParameter) {}
        }
    }
}