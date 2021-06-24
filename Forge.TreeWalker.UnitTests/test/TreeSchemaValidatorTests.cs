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
        private string stringSchema;
        private string stringSchemaList;
        private ForgeTree treeSchema;
        private Dictionary<string, ForgeTree> treeSchemas;
        private string schemaPath;
        private string schemaDirectoryPath;
        private string stringRules;
        private JSchema jschemaRules;
        private string invalidSchemaNotTree;
        private string invalidSchemaDirectoryPath;
        private string invalidSchemaWithErrorContent;

        [TestInitialize]
        public void TestInitialize()
        {
            schemaPath = Path.Combine(Environment.CurrentDirectory, "test\\ExampleSchemas\\TardigradeSchema.json");
            schemaDirectoryPath = Path.Combine(Environment.CurrentDirectory, "test\\ExampleSchemas");
            stringRules = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "contracts\\ForgeSchemaValidationRules.json"));
            jschemaRules = JSchema.Parse(stringRules);
            stringSchema = File.ReadAllText(schemaPath);
            stringSchemaList = "{ Tree1:" + stringSchema + ", Tree2:" + stringSchema + "}";
            treeSchema =  JsonConvert.DeserializeObject<ForgeTree>((string)stringSchema);
            treeSchemas = new Dictionary<string, ForgeTree>();
            treeSchemas.Add("tree1", treeSchema);
            treeSchemas.Add("tree2", treeSchema);
            treeSchemas.Add("tree3", treeSchema);
            invalidSchemaNotTree = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "test\\InvalidTestSchemas\\InvalidTestSchema.json"));
            invalidSchemaWithErrorContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "test\\InvalidTestSchemas\\InvalidTestSchemaErrorContent.json"));
            invalidSchemaDirectoryPath = "test\\ExampleSchemas\\TardigradeSchema.json";
        }

        [TestMethod]
        public void Test_ValidateSchemaInForgeTreeWithCustomRulesInString()
        {
            bool res = ForgeSchemaValidator.ValidateSchema(treeSchema, stringRules, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaInForgeTreeWithCustomRulesInJSchema()
        {
            bool res = ForgeSchemaValidator.ValidateSchema(treeSchema, jschemaRules, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaInForgeTreeListWithCustomRulesInString()
        {
            bool res = ForgeSchemaValidator.ValidateSchema(treeSchema, stringRules, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaInForgeTreeListWithCustomRulesInJSchema()
        {
            bool res = ForgeSchemaValidator.ValidateSchema(treeSchema, jschemaRules, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaInStringWithCustomRules()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaString(stringSchema, stringRules, false, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaInStringWithCustomRulesInJSchema()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaString(stringSchema, jschemaRules, false, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemasInStringListWithCustomRulesInString()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaString(stringSchemaList, stringRules, false, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemasInStringListWithCustomRulesInJSchema()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaString(stringSchemaList, jschemaRules, false, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaFromPathWithCustomRulesInString()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaInPath(schemaPath, stringRules, false, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaFromPathWithCustomRulesInJSchema()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaInPath(schemaPath, jschemaRules, false, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemasFromDirectoryListWithCustomRulesInString()
        {
            bool res = ForgeSchemaValidator.ValidateMultipleSchemasInPath(schemaDirectoryPath, stringRules, false, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemasFromDirectoryListWithCustomRulesInJSchema()
        {
            bool res = ForgeSchemaValidator.ValidateMultipleSchemasInPath(schemaDirectoryPath, jschemaRules, false, out List<string> errorList);
            Assert.AreEqual(true, res); 
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateInvalidSchemaInString()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaString(stringSchema, jschemaRules, false, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateInvalidSchemaDirectoryPath()
        {
            bool res = ForgeSchemaValidator.ValidateMultipleSchemasInPath(invalidSchemaDirectoryPath, jschemaRules, false, out List<string> errorList);
            Assert.AreEqual(false, res);
            Assert.AreEqual("The directory name is invalid.\r\n", errorList.First());
        }

        [TestMethod]
        public void Test_ValidateSchemaWithErrorContent()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaString(invalidSchemaWithErrorContent, jschemaRules, false, out List<string> errorList);
            Assert.AreEqual(false, res);
            Assert.AreEqual("JSON is valid against no schemas from 'oneOf'. line: 3, position: 17", errorList.First());
        }

        [TestMethod]
        public void Test_ValidateInvalidSchemaAsDictionary()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaString(invalidSchemaWithErrorContent, jschemaRules, true, out List<string> errorList);
            Assert.AreEqual(false, res);
            Assert.AreEqual("JSON is valid against no schemas from 'oneOf'. line: 3, position: 17", errorList.First());
        }

        [TestMethod]
        public void Test_ValidateValidSchemaAsDictionary()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaString(invalidSchemaWithErrorContent, jschemaRules, true, out List<string> errorList);
            Assert.AreEqual(false, res);
            Assert.AreEqual("JSON is valid against no schemas from 'oneOf'. line: 3, position: 17", errorList.First());
        }

        [TestMethod]
        public void Test_ValidateSchemasFromDirectoryListAsDictionaryWithCustomRulesInString()
        {
            bool res = ForgeSchemaValidator.ValidateMultipleSchemasInPath(schemaDirectoryPath, stringRules, true, out List<string> errorList);
            Assert.AreEqual(false, res);
            Assert.AreEqual("An item with the same key has already been added.", errorList.First());
        }

        [TestMethod]
        public void Test_GetLinkedJSchemaRules()
        {
            try
            {
                JSchema linkedRules = ForgeSchemaValidator.GetLinkedJSchemaRules(stringRules, stringRules, "//ForgeSchemaValidationRules.json");
                ForgeSchemaValidator.ValidateSchema(treeSchema, linkedRules, out List<string> errorList);
                Assert.AreEqual(0, errorList.Count);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }      
        }
    }
}
