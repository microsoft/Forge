//-----------------------------------------------------------------------
// <copyright file="TreeWalkerUnitTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Tests the TreeWalkerSession class.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.TreeWalker.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Microsoft.Forge.DataContracts;
    using Microsoft.Forge.TreeWalker;
    using Microsoft.Forge.TreeWalker.ForgeExceptions;
    using Newtonsoft.Json;

    [TestClass]
    public class TreeWalkerUnitTests
    {
        private const string TardigradeSchemaPath = "test\\ExampleSchemas\\TardigradeSchema.json";
        private const string TestEvaluateInputTypeSchemaPath = "test\\ExampleSchemas\\TestEvaluateInputTypeSchema.json";
        private const string LeafNodeSummarySchemaPath = "test\\ExampleSchemas\\LeafNodeSummarySchema.json";
        private const string SubroutineSchemaPath = "test\\ExampleSchemas\\SubroutineSchema.json";

        private Guid sessionId;
        private IForgeDictionary forgeState = new ForgeDictionary(new Dictionary<string, object>(), Guid.Empty, Guid.Empty);
        private dynamic UserContext = new System.Dynamic.ExpandoObject();
        private ITreeWalkerCallbacks callbacks;
        private CancellationToken token;
        private TreeWalkerParameters parameters;
        private TreeWalkerSession session;
        private Dictionary<string, ForgeTree> forgeTrees = new Dictionary<string, ForgeTree>();

        public void TestInitialize(string jsonSchema, string treeName = null)
        {
            // Initialize contexts, callbacks, and actions.
            this.sessionId = Guid.NewGuid();
            this.forgeState = new ForgeDictionary(new Dictionary<string, object>(), this.sessionId, this.sessionId);
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
                ForgeActionsAssembly = typeof(CollectDiagnosticsAction).Assembly,
                InitializeSubroutineTree = this.InitializeSubroutineTree,
                TreeName = treeName
            };

            this.session = new TreeWalkerSession(this.parameters);
        }

        public void TestSubroutineInitialize(string jsonSchema, string treeName = "RootTree")
        {
            // Subroutine tests use a ForgeSchema file that deserializes to a Dictionary of TreeName to ForgeTree.
            this.forgeTrees = JsonConvert.DeserializeObject<Dictionary<string, ForgeTree>>(jsonSchema);
            ForgeTree forgeTree = this.forgeTrees[treeName];
            string rootSchema = JsonConvert.SerializeObject(forgeTree);

            this.TestInitialize(rootSchema, treeName);
        }

        public void TestFromFileInitialize(string filePath, string treeName = null)
        {
            string jsonSchema = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, filePath));
            if (treeName == null)
            {
                this.TestInitialize(jsonSchema, treeName);
            }
            else
            {
                this.TestSubroutineInitialize(jsonSchema, treeName);
            }
        }

        [TestMethod]
        public void TestTreeWalkerSession_Constructor()
        {
            this.TestFromFileInitialize(filePath: TardigradeSchemaPath);

            // Test 1 - Verify jsonSchema was successfully deserialized in constructor.
            Assert.AreEqual("Action", this.session.Schema.Tree["Tardigrade"].Type.ToString());

            // Test 2 - Verify the Status is Initialized.
            Assert.AreEqual("Initialized", this.session.Status, "Expected WalkTree status to be Initialized after initializing TreeWalkerSession.");
        }

        [TestMethod]
        public void TestTreeWalkerSession_VisitNode_Success()
        {
            this.TestFromFileInitialize(filePath: TardigradeSchemaPath);

            // Test - VisitNode and expect the first child to be returned.
            string expected = "Tardigrade";
            string actualNextTreeNodeKey = this.session.VisitNode("Container").GetAwaiter().GetResult();

            Assert.AreEqual(expected, actualNextTreeNodeKey, "Expected VisitNode(Container) to return Tardigrade.");
        }

        [TestMethod]
        public void TestTreeWalkerSession_VisitNode_LeafNode_Success()
        {
            this.TestFromFileInitialize(filePath: TardigradeSchemaPath);

            // Test - VisitNode on node of Leaf type and confirm it does not throw.
            string expected = null;
            string actualNextTreeNodeKey = this.session.VisitNode("Tardigrade_Success").GetAwaiter().GetResult();

            Assert.AreEqual(expected, actualNextTreeNodeKey, "Expected VisitNode(Tardigrade_Success) to return without throwing.");
        }

        [TestMethod]
        public void TestTreeWalkerSession_VisitNode_NoTimeout_Success()
        {
            this.TestFromFileInitialize(filePath: TardigradeSchemaPath);

            // Test - VisitNode with no Timeout and execute an Action with no Timeout set. Confirm we do not throw exceptions.
            string expected = "Tardigrade_Success";
            string actualNextTreeNodeKey = this.session.VisitNode("Tardigrade").GetAwaiter().GetResult();
            Assert.AreEqual(expected, actualNextTreeNodeKey, "Expected VisitNode(Tardigrade) to return Tardigrade_Success without throwing exception.");
        }

        [TestMethod]
        public void TestTreeWalkerSession_WalkTree_Success()
        {
            this.TestFromFileInitialize(filePath: TardigradeSchemaPath);

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
            this.TestFromFileInitialize(filePath: TardigradeSchemaPath);

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
            this.TestFromFileInitialize(filePath: TardigradeSchemaPath);

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
            this.TestFromFileInitialize(filePath: TardigradeSchemaPath);

            // Test 1 - Confirm GetCurrentTreeNode returns null before walking tree.
            Assert.AreEqual(null, this.session.GetCurrentTreeNode().GetAwaiter().GetResult(), "Expected CurrentTreeNode to be null before starting walk tree.");

            // Test 2 - Confirm GetCurrentTreeNode returns last node visited after walking tree.
            this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("Tardigrade_Success", this.session.GetCurrentTreeNode().GetAwaiter().GetResult(), "Expected CurrentTreeNode to equal the last node visited.");
        }

        [TestMethod]
        public void TestGetLastTreeAction()
        {
            this.TestFromFileInitialize(filePath: TardigradeSchemaPath);

            // Test 1 - Confirm GetLastTreeAction returns null before walking tree.
            Assert.AreEqual(null, this.session.GetLastTreeAction().GetAwaiter().GetResult(), "Expected LastTreeAction to be null before starting walk tree.");

            // Test 2 - Confirm GetLastTreeAction returns last tree action executed after walking tree.
            this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("Tardigrade_TardigradeAction", this.session.GetLastTreeAction().GetAwaiter().GetResult(), "Expected LastTreeAction to equal the last tree action executed.");
        }

        [TestMethod]
        public void TestGetOutput()
        {
            this.TestFromFileInitialize(filePath: TardigradeSchemaPath);

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

        [TestMethod]
        public void Test_EvaluateInputType_Success()
        {
            this.TestFromFileInitialize(filePath: TestEvaluateInputTypeSchemaPath);

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
            this.TestFromFileInitialize(filePath: LeafNodeSummarySchemaPath);

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

        [TestMethod]
        public void Test_SubroutineAction_ConfirmLastActionResponseGetsPersisted_Success()
        {
            this.TestFromFileInitialize(filePath: SubroutineSchemaPath, treeName: "ParentTree");

            // Test - WalkTree to execute a SubroutineAction. Subroutine tree contains an action, defines a RootTreeNodeKey, and queries TreeInput from the schema.
            //        Confirm the output of the SubroutineAction is the last ActionResponse in the Subroutine tree.
            string actualStatus = this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("RanToCompletion", actualStatus);

            ActionResponse subroutineActionResponse = this.session.GetOutput("Root_Subroutine");
            Assert.AreEqual(
                "Success",
                subroutineActionResponse.Status,
                "Expected to successfully retrieve the output value from the action that matches the last action response of the subroutine tree.");

            Assert.AreEqual(
                10,
                subroutineActionResponse.StatusCode,
                "Expected to successfully retrieve the output value from the action that matches the last action response of the subroutine tree.");
        }

        [TestMethod]
        public void Test_SubroutineAction_NoActions_Success()
        {
            this.TestSubroutineInitialize(jsonSchema: ForgeSchemaHelper.SubroutineAction_NoActions, treeName: "RootTree");

            // Test - WalkTree to execute a SubroutineAction. Subroutine tree contains no Actions. Subroutine tree does not specify RootTreeNodeKey, so expect to visit "Root" be default.
            //        Confirm the output of the SubroutineAction is the Status of the Subroutine tree walker session.
            string actualStatus = this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("RanToCompletion", actualStatus);

            ActionResponse subroutineActionResponse = this.session.GetOutput("Root_Subroutine");
            Assert.AreEqual(
                "RanToCompletion",
                subroutineActionResponse.Status,
                "Expected to successfully retrieve the Status of the subroutine session, since the subroutine tree contained no Actions.");
        }

        [TestMethod]
        public void Test_SubroutineAction_ConfirmIntermediatesUsePersistedSessionIdOnRehydration_Success()
        {
            this.TestSubroutineInitialize(jsonSchema: ForgeSchemaHelper.SubroutineAction_NoActions, treeName: "RootTree");

            // WalkTree to execute a SubroutineAction.
            string actualStatus = this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("RanToCompletion", actualStatus);

            ActionResponse subroutineActionResponse = this.session.GetOutput("Root_Subroutine");
            Assert.AreEqual(
                "RanToCompletion",
                subroutineActionResponse.Status,
                "Expected to successfully retrieve the Status of the subroutine session, since the subroutine tree contained no Actions.");

            // Cache the original subroutine SessionId to check against later.
            SubroutineIntermediates subroutineIntermediates = this.forgeState.GetValue<SubroutineIntermediates>("Root_Subroutine" + TreeWalkerSession.IntermediatesSuffix).GetAwaiter().GetResult();
            Guid subroutineSessionId = subroutineIntermediates.SessionId;

            // Brain surgery to make it look like we failed over during SubroutineAction before the ActionResponse was persisted.
            this.forgeState.Set<ActionResponse>("Root_Subroutine" + TreeWalkerSession.ActionResponseSuffix, null).GetAwaiter().GetResult();
            this.session = new TreeWalkerSession(this.session.Parameters);

            actualStatus = this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("RanToCompletion", actualStatus);

            // Test - Confirm the SubroutineIntermediates.SessionId is persisted and gets re-used on rehydration.
            subroutineIntermediates = this.forgeState.GetValue<SubroutineIntermediates>("Root_Subroutine" + TreeWalkerSession.IntermediatesSuffix).GetAwaiter().GetResult();
            Assert.AreEqual(subroutineSessionId, subroutineIntermediates.SessionId);
        }

        [TestMethod]
        public void Test_SubroutineAction_ParallelSubroutineActions_Success()
        {
            this.TestSubroutineInitialize(jsonSchema: ForgeSchemaHelper.SubroutineAction_ParallelSubroutineActions, treeName: "RootTree");

            // Test - WalkTree to execute a Subroutine node with 2 SubroutineActions and a regular Action in parallel.
            //        Confirm parallel actions execute successfully.
            string actualStatus = this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("RanToCompletion", actualStatus);

            ActionResponse subroutineActionResponse = this.session.GetOutput("Root_Subroutine_One");
            Assert.AreEqual(
                "TestValueOne",
                subroutineActionResponse.Status,
                "Expected to successfully retrieve the output value from the action that matches the last action response of the subroutine tree.");

            subroutineActionResponse = this.session.GetOutput("Root_Subroutine_Two");
            Assert.AreEqual(
                "TestValueTwo",
                subroutineActionResponse.Status,
                "Expected to successfully retrieve the output value from the action that matches the last action response of the subroutine tree.");

            ActionResponse actionResponse = this.session.GetOutput("Root_CollectDiagnosticsAction");
            Assert.AreEqual(
                "Success",
                actionResponse.Status,
                "Expected to successfully read ActionResponse.Status.");
        }

        [TestMethod]
        public void Test_SubroutineAction_FailsOnActionTreeNodeType_Failure()
        {
            this.TestSubroutineInitialize(jsonSchema: ForgeSchemaHelper.SubroutineAction_FailsOnActionTreeNodeType, treeName: "RootTree");

            // Test - WalkTree and fail to execute an Action type node containing a SubroutineAction.
            string actual;
            Assert.ThrowsException<ArgumentException>(() =>
            {
                actual = this.session.WalkTree("Root").GetAwaiter().GetResult();
            }, "Expected WalkTree to fail because the schema contained a Property that does not exist in ActionDefinition.InputType.");

            actual = this.session.Status;
            Assert.AreEqual(
                "Failed",
                actual,
                "Expected WalkTree to fail because the schema contained a Property that does not exist in ActionDefinition.InputType.");
        }

        [TestMethod]
        public void Test_SubroutineAction_FailsOnNoSubroutineAction_Failure()
        {
            this.TestSubroutineInitialize(jsonSchema: ForgeSchemaHelper.SubroutineAction_FailsOnNoSubroutineAction, treeName: "RootTree");

            // Test - WalkTree and fail to execute a Subroutine type node that does not contain at least one SubroutineAction.
            string actual;
            Assert.ThrowsException<ArgumentException>(() =>
            {
                actual = this.session.WalkTree("Root").GetAwaiter().GetResult();
            }, "Expected WalkTree to fail because the schema contained a Property that does not exist in ActionDefinition.InputType.");

            actual = this.session.Status;
            Assert.AreEqual(
                "Failed",
                actual,
                "Expected WalkTree to fail because the schema contained a Property that does not exist in ActionDefinition.InputType.");
        }

        [TestMethod]
        public void Test_Cycles_Success()
        {
            this.TestInitialize(jsonSchema: ForgeSchemaHelper.CycleSchema);

            // Test - WalkTree that revisits a node multiple times.
            //        Inside the Action, we confirm that GetPreviousActionResponse gets persisted and Action Intermediates get wiped.
            string actualStatus = this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("RanToCompletion_NoChildMatched", actualStatus);

            ActionResponse actionResponse = this.session.GetLastActionResponse();
            Assert.AreEqual(
                3,
                (int)actionResponse.Output,
                "Expected to successfully retrieve the output value from the action that matches the last action response of the subroutine tree.");
        }

        [TestMethod]
        public void Test_Cycles_RevisitSubroutineActionUsesDifferentSessionId_Success()
        {
            this.TestSubroutineInitialize(jsonSchema: ForgeSchemaHelper.Cycle_SubroutineActionUsesDifferentSessionId);

            // Test - WalkTree that revisits a Subroutine node multiple times.
            //        Confirm different SessionIds get used each time we revisit the SubroutineAction.
            string actualStatus = this.session.WalkTree("Root").GetAwaiter().GetResult();
            Assert.AreEqual("RanToCompletion_NoChildMatched", actualStatus);

            string rootSessionId = this.session.Parameters.RootSessionId.ToString();
            ActionResponse previousActionResponse = this.forgeState.GetValue<ActionResponse>("Root_Subroutine" + TreeWalkerSession.PreviousActionResponseSuffix).GetAwaiter().GetResult();
            ActionResponse actionResponse = this.session.GetOutput("Root_Subroutine");

            Assert.AreNotEqual(
                previousActionResponse.Status,
                actionResponse.Status,
                "Expected to successfully retrieve the output value from the action that matches the last action response of the subroutine tree.");

            Assert.AreNotEqual(
                rootSessionId,
                previousActionResponse.Status);

            Assert.AreNotEqual(
                rootSessionId,
                actionResponse.Status);
        }

        /// <summary>
        /// Used to test ExternalExecutors.
        /// </summary>
        private static async Task<object> External(string expression, CancellationToken token)
        {
            // External executes the expression and returns.
            await Task.Delay(1, token);
            return expression + "_Executed";
        }

        /// <summary>
        /// Used to test SubroutineActions.
        /// </summary>
        private TreeWalkerSession InitializeSubroutineTree(SubroutineInput subroutineInput, Guid subroutineSessionId, TreeWalkerParameters parentParameters)
        {
            string jsonSchema = JsonConvert.SerializeObject(forgeTrees[subroutineInput.TreeName]);
            TreeWalkerParameters subroutineParameters = new TreeWalkerParameters(
                subroutineSessionId,
                jsonSchema,
                new ForgeDictionary(new Dictionary<string, object>(), parentParameters.RootSessionId, subroutineSessionId),
                this.callbacks,
                this.token)
            {
                UserContext = this.UserContext,
                ForgeActionsAssembly = typeof(CollectDiagnosticsAction).Assembly,
                InitializeSubroutineTree = this.InitializeSubroutineTree,
                RootSessionId = parentParameters.RootSessionId,
                TreeName = subroutineInput.TreeName,
                TreeInput = subroutineInput.TreeInput
            };

            return new TreeWalkerSession(subroutineParameters);
        }
    }
}