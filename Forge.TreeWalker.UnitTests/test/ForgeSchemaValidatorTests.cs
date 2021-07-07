//-----------------------------------------------------------------------
// <copyright file="ForgeSchemaValidatorTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Tests for the ForgeSchemaValidatorTests class.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.TreeWalker.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Forge.DataContracts;
    using Microsoft.Forge.TreeWalker;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Schema;

    [TestClass]
    public class ForgeSchemaValidatorTests
    {
        private string rulesForForgeTree;
        private string rulesForForgeTreeDictionary;
        private JSchema jSchemaRulesForForgeTree;
        private JSchema linkedRulesForForgeTreeDictionary;
        private string pathToForgeTree;
        private string directoryPathToMultipleForgeTree;
        private string forgeTreeAsString;
        private ForgeTree forgeTree;
        private string forgeTreeDictionaryAsString;
        private Dictionary<string, ForgeTree> forgeTreeDictionary;
        private string invalidSchemaWithErrorContent;
        private string invalidSchemaDirectoryPath;

        [TestInitialize]
        public void TestInitialize()
        {
            rulesForForgeTree = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "contracts\\ForgeSchemaValidationRules.json"));
            rulesForForgeTreeDictionary = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "contracts\\ForgeSchemaDictionaryValidationRules.json"));
            jSchemaRulesForForgeTree = JSchema.Parse(rulesForForgeTree);

            linkedRulesForForgeTreeDictionary = ForgeSchemaValidator.GetLinkedJSchemaRules(rulesForForgeTreeDictionary, rulesForForgeTree, "//ForgeSchemaValidationRules.json");

            pathToForgeTree = Path.Combine(Environment.CurrentDirectory, "test\\ExampleSchemas\\TardigradeSchema.json");
            directoryPathToMultipleForgeTree = Path.Combine(Environment.CurrentDirectory, "test\\ExampleSchemas");

            forgeTreeAsString = File.ReadAllText(pathToForgeTree);
            forgeTree = JsonConvert.DeserializeObject<ForgeTree>((string)forgeTreeAsString);

            forgeTreeDictionaryAsString = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "test\\ExampleSchemas\\SubroutineSchema.json"));
            forgeTreeDictionary = new Dictionary<string, ForgeTree>();
            forgeTreeDictionary.Add("tree1", forgeTree);
            forgeTreeDictionary.Add("tree2", forgeTree);
            forgeTreeDictionary.Add("tree3", forgeTree);
            
            invalidSchemaWithErrorContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "test\\InvalidTestSchemas\\InvalidTestSchemaErrorContent.json"));
            invalidSchemaDirectoryPath = "test\\ExampleSchemas\\TardigradeSchema.json";
        }

        [TestMethod]
        public void Test_GetLinkedJSchemaRules()
        {
            try
            {
                JSchema linkedRules = ForgeSchemaValidator.GetLinkedJSchemaRules(rulesForForgeTreeDictionary, rulesForForgeTree, "//ForgeSchemaValidationRules.json");
                bool res = ForgeSchemaValidator.ValidateSchemaAsForgeTreeDictionary(forgeTreeDictionary, linkedRules, true, out IList<ValidationError> errorList);
                Assert.IsTrue(res);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void Test_ValidateSchemaAsForgeTree_WithRulesAsString()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsForgeTree(forgeTree, rulesForForgeTree, out IList<ValidationError> errorList);
            Assert.IsTrue(res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaAsForgeTree_WithRulesAsJSchema()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsForgeTree(forgeTree, jSchemaRulesForForgeTree, out IList<ValidationError> errorList);
            Assert.IsTrue(res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaAsForgeTreeDictionary_WithoutValidateAsDictionary()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsForgeTreeDictionary(forgeTreeDictionary, rulesForForgeTree, false, out IList<ValidationError> errorList);
            Assert.IsTrue(res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaAsForgeTreeDictionary_WithValidateAsDictionary()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsForgeTreeDictionary(forgeTreeDictionary, linkedRulesForForgeTreeDictionary, true, out IList<ValidationError> errorList);
            Assert.IsTrue(res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaAsString_WithForgeTree()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsString(forgeTreeAsString, rulesForForgeTree, false, out IList<ValidationError> errorList);
            Assert.IsTrue(res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaAsString_WithValidateAsDictionary()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsString(forgeTreeDictionaryAsString, linkedRulesForForgeTreeDictionary, true, out IList<ValidationError> errorList);
            Assert.IsTrue(res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaFromPath_WithoutValidateAsDictionary()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaFromPath(pathToForgeTree, rulesForForgeTree, false, out IList<ValidationError> errorList);
            Assert.IsTrue(res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemasFromDirectory_WithValidateAsSeparateFiles()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaFromDirectory(directoryPathToMultipleForgeTree, rulesForForgeTree, false, out IList<ValidationError> errorList);
            Assert.IsTrue(res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_GetLinkedJSchemaRules_WithInvalidUrl_Fail()
        {
            Assert.ThrowsException<JSchemaReaderException>(() => ForgeSchemaValidator.GetLinkedJSchemaRules(rulesForForgeTreeDictionary, rulesForForgeTree, "//nameImadeup.json"));
        }

        [TestMethod]
        public void Test_ValidateSchemaAsForgeTree_WithEmptyStringRules_Fail()
        {
            Assert.ThrowsException<ArgumentException>(() => ForgeSchemaValidator.ValidateSchemaAsForgeTree(forgeTree, "", out IList<ValidationError> errorList));
        }

        [TestMethod]
        public void Test_ValidateSchemaAsString_WithErrorContent_Fail()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsString(invalidSchemaWithErrorContent, jSchemaRulesForForgeTree, false, out IList<ValidationError> errorList);
            Assert.AreEqual(false, res);
            Assert.AreEqual("JSON is valid against no schemas from 'oneOf'.", errorList.First().Message);
        }

        [TestMethod]
        public void Test_ValidateSchema_FromInvalidDirectoryPath_Fail()
        {
            Assert.ThrowsException<IOException>(
                () => ForgeSchemaValidator.ValidateSchemaFromDirectory(
                          invalidSchemaDirectoryPath,
                          linkedRulesForForgeTreeDictionary,
                          true,
                          out IList<ValidationError> errorList));
        }

        [TestMethod]
        public void Test_ValidateSchemaFromDirectory_WithValidateAsDictionary_DirectoryContainsForgeTree_Fail()
        {
            Assert.ThrowsException<NullReferenceException>(
                () => ForgeSchemaValidator.ValidateSchemaFromDirectory(
                          directoryPathToMultipleForgeTree,
                          linkedRulesForForgeTreeDictionary,
                          true,
                          out IList<ValidationError> errorList));
        }

        [TestMethod]
        public void Test_ValidateSchemaAsForgeTreeDictionary_WithRulesUnlinked_Fail()
        {
            Assert.ThrowsException<JSchemaReaderException>(
                () => ForgeSchemaValidator.ValidateSchemaAsForgeTreeDictionary(
                          forgeTreeDictionary, 
                          rulesForForgeTreeDictionary, 
                          true, 
                          out IList<ValidationError> errorList));
        }
    }
}
