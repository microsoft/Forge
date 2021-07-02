//-----------------------------------------------------------------------
// <copyright file="TreeSchemaValidatorTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Tests for the TreeSchemaValidator class.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.TreeWalker.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Forge.DataContracts;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Schema;
    using Microsoft.Forge.TreeWalker;

    [TestClass]
    public class TreeSchemaValidatorTests
    {
        private string forgeTreeAsString;
        private ForgeTree treeSchema;
        private Dictionary<string, ForgeTree> treeSchemas;
        private string forgeTreeFromPath;
        private string schemaDirectoryPath;
        private string stringRules;
        private string rulesForDictionary;
        private JSchema linkedRulesForDictionary;
        private JSchema jschemaRules;
        private string invalidSchemaDirectoryPath;
        private string invalidSchemaWithErrorContent;
        private string forgeTreeDictionaryAsString;

        [TestInitialize]
        public void TestInitialize()
        {
            forgeTreeFromPath = Path.Combine(Environment.CurrentDirectory, "test\\ExampleSchemas\\TardigradeSchema.json");
            schemaDirectoryPath = Path.Combine(Environment.CurrentDirectory, "test\\ExampleSchemas");
            stringRules = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "contracts\\ForgeSchemaValidationRules.json"));
            rulesForDictionary = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "contracts\\ForgeSchemaDictionaryValidationRules.json"));
            linkedRulesForDictionary = ForgeSchemaValidator.GetLinkedJSchemaRules(rulesForDictionary, stringRules, "//ForgeSchemaValidationRules.json");
            jschemaRules = JSchema.Parse(stringRules);
            forgeTreeAsString = File.ReadAllText(forgeTreeFromPath);
            forgeTreeDictionaryAsString = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "test\\ExampleSchemas\\SubroutineSchema.json"));
            treeSchema = JsonConvert.DeserializeObject<ForgeTree>((string)forgeTreeAsString);
            treeSchemas = new Dictionary<string, ForgeTree>();
            treeSchemas.Add("tree1", treeSchema);
            treeSchemas.Add("tree2", treeSchema);
            treeSchemas.Add("tree3", treeSchema);
            invalidSchemaWithErrorContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "test\\InvalidTestSchemas\\InvalidTestSchemaErrorContent.json"));
            invalidSchemaDirectoryPath = "test\\ExampleSchemas\\TardigradeSchema.json";
        }

        [TestMethod]
        public void Test_GetLinkedJSchemaRules()
        {
            try
            {
                JSchema linkedRules = ForgeSchemaValidator.GetLinkedJSchemaRules(rulesForDictionary, stringRules, "//ForgeSchemaValidationRules.json");
                bool res = ForgeSchemaValidator.ValidateSchemaAsForgeTreeDictionary(treeSchemas, linkedRules, true, out IList<ValidationError> errorList);
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
            bool res = ForgeSchemaValidator.ValidateSchemaAsForgeTree(treeSchema, stringRules, out IList<ValidationError> errorList);
            Assert.IsTrue(res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaAsForgeTree_WithRulesAsJSchema()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsForgeTree(treeSchema, jschemaRules, out IList<ValidationError> errorList);
            Assert.IsTrue(res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaAsForgeTreeDictionary_WithoutValidateAsDictionary()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsForgeTreeDictionary(treeSchemas, stringRules, false, out IList<ValidationError> errorList);
            Assert.IsTrue(res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaAsForgeTreeDictionary_WithValidateAsDictionary()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsForgeTreeDictionary(treeSchemas, linkedRulesForDictionary, true, out IList<ValidationError> errorList);
            Assert.IsTrue(res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaAsString_WithForgeTree()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsString(forgeTreeAsString, stringRules, false, out IList<ValidationError> errorList);
            Assert.IsTrue(res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaAsString_WithValidateAsDictionary()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsString(forgeTreeDictionaryAsString, linkedRulesForDictionary, true, out IList<ValidationError> errorList);
            Assert.IsTrue(res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaFromPath_WithoutValidateAsDictionary()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaFromPath(forgeTreeFromPath, stringRules, false, out IList<ValidationError> errorList);
            Assert.IsTrue(res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemasFromDirectory_WithValidateAsSeparateFiles()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaFromDirectory(schemaDirectoryPath, stringRules, false, out IList<ValidationError> errorList);
            Assert.IsTrue(res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_GetLinkedJSchemaRules_WithInvalidUrl__Fail()
        {
            Assert.ThrowsException<JSchemaReaderException>(() => ForgeSchemaValidator.GetLinkedJSchemaRules(rulesForDictionary, stringRules, "//nameImadeup.json"));
        }

        [TestMethod]
        public void Test_ValidateSchemaAsForgeTree_WithEmptyStringRules__Fail()
        {
            Assert.ThrowsException<ArgumentException>(() => ForgeSchemaValidator.ValidateSchemaAsForgeTree(treeSchema, "", out IList<ValidationError> errorList));
        }

        [TestMethod]
        public void Test_ValidateSchemaAsString_WithErrorContent_Fail()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsString(invalidSchemaWithErrorContent, jschemaRules, false, out IList<ValidationError> errorList);
            Assert.AreEqual(false, res);
            Assert.AreEqual("JSON is valid against no schemas from 'oneOf'.", errorList.First().Message);
        }

        [TestMethod]
        public void Test_ValidateSchema_FromInvalidDirectoryPath_Fail()
        {
            Assert.ThrowsException<IOException>(() => ForgeSchemaValidator.ValidateSchemaFromDirectory(invalidSchemaDirectoryPath, jschemaRules, false, out IList<ValidationError> errorList));
        }

        [TestMethod]
        public void Test_ValidateSchemaFromDirectory_WithValidateAsDictionary_DirectoryContainsForgeTree_Fail()
        {
            Assert.ThrowsException<NullReferenceException>(() => ForgeSchemaValidator.ValidateSchemaFromDirectory(schemaDirectoryPath, linkedRulesForDictionary, true, out IList<ValidationError> errorList));
        }

        [TestMethod]
        public void Test_ValidateSchemaAsForgeTreeDictionary_WithRulesUnlinked_Fail()
        {
            Assert.ThrowsException<JSchemaReaderException>(() => ForgeSchemaValidator.ValidateSchemaAsForgeTreeDictionary(treeSchemas, rulesForDictionary, true, out IList<ValidationError> errorList));
        }
    }
}
