//-----------------------------------------------------------------------
// <copyright file="ExecutorUnitTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Tests the ExpressionExecutor class.
// </summary>
//-----------------------------------------------------------------------

namespace Forge.TreeWalker.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Forge.ExternalTypes;
    using Forge.TreeWalker;

    [TestClass]
    public class ExecutorUnitTests
    {
        private dynamic UserContext;

        [TestInitialize]
        public void TestInitialize()
        {
            this.UserContext = new System.Dynamic.ExpandoObject();
            this.UserContext.GetTopic = (Func<string, dynamic>)((topicName) =>
            {
                dynamic result = new System.Dynamic.ExpandoObject();
                result.ResourceType = "Node";
                return result;
            });
            
            this.UserContext.GetCount = (Func<int>)(() =>
            {
                return 1;
            });
        }

        [TestMethod]
        public void TestExecutor_Success_bool()
        {
            this.UserContext.Foo = "Bar";
            ExpressionExecutor ex = new ExpressionExecutor(null, this.UserContext, null);
            string expression = "UserContext.Foo == \"Bar\"";

            Assert.IsTrue(ex.Execute<bool>(expression).GetAwaiter().GetResult(), "Expected ExpressionExecutor to successfully evaluate a true expression.");
        }

        [TestMethod]
        public void TestExecutor_Success_long()
        {
            // Note that long requires casting where int does not.
            this.UserContext.Foo = (long)1000;
            ExpressionExecutor ex = new ExpressionExecutor(null, this.UserContext, null);
            string expression = "UserContext.Foo";

            Assert.AreEqual((long)1000, ex.Execute<long>(expression).GetAwaiter().GetResult(), "Expected ExpressionExecutor to successfully evaluate the expression.");
        }

        [TestMethod]
        public void TestExecutor_Success_int()
        {
            this.UserContext.Foo = 1000;
            ExpressionExecutor ex = new ExpressionExecutor(null, this.UserContext, null);
            string expression = "UserContext.Foo";

            Assert.AreEqual(1000, ex.Execute<int>(expression).GetAwaiter().GetResult(), "Expected ExpressionExecutor to successfully evaluate the expression.");
        }

        [TestMethod]
        public void TestExecutor_Success_ExecuteTwice()
        {
            this.UserContext.Foo = "Bar";
            ExpressionExecutor ex = new ExpressionExecutor(null, this.UserContext, null);
            string expression = "UserContext.Foo == \"Bar\" && UserContext.GetTopic(\"TopicName\").ResourceType == \"Node\"";

            // Test - confirm Execute can compile and execute the same code twice without crashing.
            Assert.IsTrue(ex.Execute<bool>(expression).GetAwaiter().GetResult(), "Expected ExpressionExecutor to successfully evaluate a true expression.");
            Assert.IsTrue(ex.Execute<bool>(expression).GetAwaiter().GetResult(), "Expected ExpressionExecutor to successfully evaluate a true expression.");
        }

        [TestMethod]
        public void TestExecutor_Fail_MissingDefinitions()
        {
            this.UserContext.Foo = "Bar";
            ExpressionExecutor ex = new ExpressionExecutor(null, this.UserContext, null);
            string expression = "UserContext.Bar == \"Bar\"";

            try
            {
                ex.Execute<bool>(expression).GetAwaiter().GetResult();
                Assert.Fail("Expected ExpressionExecutor to fail evaluating an expression when UserContext does not contain a necessary definitions.");
            }
            catch (Exception)
            {
            }
        }

        [TestMethod]
        public void TestExecutor_Fail_BadExpression()
        {
            this.UserContext.Foo = "Bar";
            ExpressionExecutor ex = new ExpressionExecutor(null, this.UserContext, null);
            string expression = "UserContext.Foo";

            Assert.ThrowsException<InvalidCastException>(() => 
            {
                ex.Execute<bool>(expression).GetAwaiter().GetResult();
            }, "Expected ExpressionExecutor to fail evaluating an expression that cannot be evaluated.");
        }

        [TestMethod]
        public void TestExecutor_Success_CompileWithExternalDependencies()
        {
            this.UserContext.Foo = ExternalTestType.TestEnum;
            List<Type> dependencies = new List<Type>();
            dependencies.Add(typeof(ExternalTestType));
            ExpressionExecutor ex = new ExpressionExecutor(null, this.UserContext, dependencies);
            string expression = "UserContext.Foo == ExternalTestType.TestEnum";
            Assert.IsTrue(ex.Execute<bool>(expression).GetAwaiter().GetResult(), "Expected ExpressionExecutor to successfully evaluate a true expression.");
        }

        [TestMethod]
        public void TestExecutor_Fail_CompileExpressionWithMissingDependencies()
        {
            this.UserContext.Foo = ExternalTestType.ExampleEnum;
            ExpressionExecutor ex = new ExpressionExecutor(null, this.UserContext, null);
            string expression = "UserContext.Foo == ExternalTestType.ExampleEnum";

            try
            {
                ex.Execute<bool>(expression).GetAwaiter().GetResult();
                Assert.Fail("Expected ExpressionExecutor to fail evaluating an expression because runtime assembly is missing required dependencies.");
            }
            catch (Exception)
            {
            }
        }

        [TestMethod]
        public void TestExecutor_Fail_CompileExpressionWithExternalDependenciesAndMissingDependencies()
        {
            this.UserContext.Foo = ExternalTestType.TestEnum;
            this.UserContext.Bar = DiffNamespaceType.TestOne;
            List<Type> dependencies = new List<Type>();
            dependencies.Add(typeof(ExternalTestType));

            ExpressionExecutor ex = new ExpressionExecutor(null, this.UserContext, dependencies);
            string expression = "UserContext.Foo == ExternalTestType.TestEnum && UserContext.Bar == DiffNamespaceType.TestOne";

            try
            {
                ex.Execute<bool>(expression).GetAwaiter().GetResult();
                Assert.Fail("Expected ExpressionExecutor to fail evaluating an expression because runtime assembly is missing required dependencies.");
            }
            catch (Exception)
            {
            }
        }

        [TestMethod]
        public void TestExecutor_Success_CompileExpressionWithMissingDependenciesButOtherExternalTypesInSameNamespace()
        {
            this.UserContext.Foo = ExternalTestType.TestEnum;
            this.UserContext.Bar = TestType.Test;
            List<Type> dependencies = new List<Type>();
            dependencies.Add(typeof(ExternalTestType));

            ExpressionExecutor ex = new ExpressionExecutor(null, this.UserContext, dependencies);
            string expression = "UserContext.Foo == ExternalTestType.TestEnum && UserContext.Bar == TestType.Test";
            Assert.IsTrue(ex.Execute<bool>(expression).GetAwaiter().GetResult(), "Expected ExpressionExecutor to successfully evaluate a true expression.");
        }

        [TestMethod]
        public void TestExecutor_Success_CompileExpressionWithForgeDefaultDependenciesBeingPassedInExternally()
        {
            this.UserContext.Foo = "Foo";
            List<Type> dependencies = new List<Type>();
            // Tasks dependency needed by Forge by default.
            dependencies.Add(typeof(Task));
            // Reflection dependency needed by Forge for runtime compilation.
            dependencies.Add(typeof(Type));

            // Default dependencies are expected to be tossed away internally in ExpressionExecutor. 
            ExpressionExecutor ex = new ExpressionExecutor(null, this.UserContext, null);
            string expression = "UserContext.Foo == \"Foo\"";
            Assert.IsTrue(ex.Execute<bool>(expression).GetAwaiter().GetResult(), "Expected ExpressionExecutor to successfully evaluate a true expression.");
        }

        [TestMethod]
        public void TestExecutor_Success_ChangingFunctionDefinition()
        {
            ExpressionExecutor ex = new ExpressionExecutor(null, this.UserContext, null);

            // Rewritting GetCount to return 2
            string expression="UserContext.GetCount = new Func<int>(() => 2)";
            Assert.AreEqual(this.UserContext.GetCount(), 1);
            ex.Execute<Func<int>>(expression).GetAwaiter().GetResult();
            Assert.AreEqual(this.UserContext.GetCount(), 2);
        }

        [TestMethod]
        public void TestExecutor_Fail_ChangingFunctionReturnType()
        {
            ExpressionExecutor ex = new ExpressionExecutor(null, this.UserContext, null);

            // Changing return type of GetCount
            string expression = "UserContext.GetCount = new Func<string>(() => \"Test\")";
            Assert.ThrowsException<InvalidCastException>(() =>
            {
                // Since expected return type matches the original Func<int> type, this should throw an error
                ex.Execute<Func<int>>(expression).GetAwaiter().GetResult();
            }, "Expected ExpressionExecutor to fail evaluating can not change return type");
        }

        [TestMethod]
        public void TestExecutor_Success_ChangingFunctionReturnType()
        {
            ExpressionExecutor ex = new ExpressionExecutor(null, this.UserContext, null);

            // Changing return type of GetCount
            string expression = "UserContext.GetCount = new Func<string>(() => \"Test\")";
            Assert.AreEqual(this.UserContext.GetCount(), 1);
            // Since expected return type has been updated, this should pass
            ex.Execute<Func<string>>(expression).GetAwaiter().GetResult();
            Assert.AreEqual(this.UserContext.GetCount(), "Test");
        }

        [TestMethod]
        public void TestExecutor_Fail_ExecutingMultipleStatements()
        {
            ExpressionExecutor ex = new ExpressionExecutor(null, this.UserContext, null);

            // Changing return type of GetCount
            string expression = "int x = UserContext.GetCount() + 5; UserContext.GetCount = new Func<int>(() => x); return UserContext.GetCount()";

            try
            {
                ex.Execute<Func<int>>(expression).GetAwaiter().GetResult();
                Assert.Fail("Expected ExpressionExecutor to fail evaluating can not pass multiple statements to expression.");
            }
            catch (Exception)
            {
            }
        }

        [TestMethod]
        public void TestExecutor_Success_WaitForDelegate()
        {
            this.UserContext.Foo = "Bar";
            string expression = "(Func<bool>)(() => {return UserContext.Foo == \"Bar\";})";

            // Casting the expression to Func<bool> since the executor will return a delegate of type Func<bool>
            ExpressionExecutor ex = new ExpressionExecutor(null, this.UserContext, null);
            dynamic expressionResult = ex.Execute<Delegate>(expression).GetAwaiter().GetResult();

            if (expressionResult.GetType() == typeof(Func<bool>))
            {
                Assert.IsTrue(expressionResult());
                this.UserContext.Foo = "Far";
                Assert.IsFalse(expressionResult());
            }
            else
            {
                Assert.Fail(string.Format("Expected expression to be of type bool but was {0}", expressionResult.GetType()));
            }
        }

        
        [TestMethod]
        public void TestExecutor_Success_WaitForDelegateAsync()
        {
            this.UserContext.Foo = "Bar";
            string expression = "(Func<Task<bool>>)(() => {return Task.FromResult(UserContext.Foo == \"Bar\");})";

            // Casting the expression to Func<bool> since the executor will return a delegate of type Func<bool>
            ExpressionExecutor ex = new ExpressionExecutor(null, this.UserContext, null);
            dynamic expressionResult = ex.Execute<Delegate>(expression).GetAwaiter().GetResult();
            
            if (expressionResult.GetType() == typeof(Func<Task<bool>>))
            {
                // expressionResult() return Task<bool>, doing .GetAwaiter().GetResult() again returns bool
                Assert.IsTrue(expressionResult().GetAwaiter().GetResult());
                this.UserContext.Foo = "Far";
                Assert.IsFalse(expressionResult().GetAwaiter().GetResult());
            }
            else
            {
                Assert.Fail(string.Format("Expected expression to be of type Task<bool> but was {0}", expressionResult.GetType()));
            }
        }

        [TestMethod]
        public void TestExecutor_ScriptCacheContainsKey()
        {
            this.UserContext.Foo = "Bar";
            string expression = "UserContext.Foo == \"Bar\" && UserContext.GetTopic(\"TopicName\").ResourceType == \"Node\"";

            // Test - confirm ExpressionExecutor script cache does not contain script before executing.
            ExpressionExecutor ex = new ExpressionExecutor(null, this.UserContext, null);
            Assert.IsFalse(ex.ScriptCacheContainsKey(expression));

            // Test - confirm ExpressionExecutor script cache does contain script after executing.
            Assert.IsTrue(ex.Execute<bool>(expression).GetAwaiter().GetResult());
            Assert.IsTrue(ex.ScriptCacheContainsKey(expression));
        }
    }
}