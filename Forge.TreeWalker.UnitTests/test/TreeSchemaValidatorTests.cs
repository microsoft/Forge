//-----------------------------------------------------------------------
// <copyright file="TreeSchemaValidatorTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Tests the TreeSchemaValidator class.
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
    using System.Threading.Tasks;

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
            var res = ForgeSchemaValidator.ValidateSchema(treeSchema, stringRules, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }
        [TestMethod]
        public void Test_ValidateSchemaInForgeTreeWithCustomRulesInJSchema()
        {
            var res = ForgeSchemaValidator.ValidateSchema(treeSchema, jschemaRules, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }
        [TestMethod]
        public void Test_ValidateSchemaInForgeTreeListWithCustomRulesInString()
        {
            var res = ForgeSchemaValidator.ValidateSchema(treeSchema, stringRules, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }
        [TestMethod]
        public void Test_ValidateSchemaInForgeTreeListWithCustomRulesInJSchema()
        {
            var res = ForgeSchemaValidator.ValidateSchema(treeSchema, jschemaRules, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }
        [TestMethod]
        public void Test_ValidateSchemaInStringWithCustomRulesInString()
        {
            var res = ForgeSchemaValidator.ValidateSchemaString(stringSchema, stringRules, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }
        [TestMethod]
        public void Test_ValidateSchemaInStringWithCustomRulesInJSchema()
        {
            var res = ForgeSchemaValidator.ValidateSchemaString(stringSchema, jschemaRules, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }
        [TestMethod]
        public void Test_ValidateSchemasInStringListWithCustomRulesInString()
        {
            var res = ForgeSchemaValidator.ValidateSchemaString(stringSchemaList, stringRules, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }
        [TestMethod]
        public void Test_ValidateSchemasInStringListWithCustomRulesInJSchema()
        {
            var res = ForgeSchemaValidator.ValidateSchemaString(stringSchemaList, jschemaRules, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaFromPathWithCustomRulesInString()
        {
            var res = ForgeSchemaValidator.ValidateSchemaInPath(schemaPath, stringRules, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }
        [TestMethod]
        public void Test_ValidateSchemaFromPathWithCustomRulesInJSchema()
        {
            var res = ForgeSchemaValidator.ValidateSchemaInPath(schemaPath, jschemaRules, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }
        [TestMethod]
        public void Test_ValidateSchemasFromDirectoryListWithCustomRulesInString()
        {
            var res = ForgeSchemaValidator.ValidateMultipleSchemasInPath(schemaDirectoryPath, stringRules, out List<string> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }
        [TestMethod]
        public void Test_ValidateSchemasFromDirectoryListWithCustomRulesInJSchema()
        {
            var res = ForgeSchemaValidator.ValidateMultipleSchemasInPath(schemaDirectoryPath, jschemaRules, out List<string> errorList);
            Assert.AreEqual(true, res); 
            Assert.AreEqual(0, errorList.Count);
        }
        [TestMethod]
        public void Test_ValidateInvalidSchemaInString()
        {
            var res = ForgeSchemaValidator.ValidateSchemaString(invalidSchemaNotTree, jschemaRules, out List<string> errorList);
            Assert.AreEqual(false, res);
            Assert.AreEqual("Required property 'Type' not found in JSON. Path 'Tree.Root', line 5, position 9.", errorList.First());
        }
        [TestMethod]
        public void Test_ValidateInvalidSchemaDirectoryPath()
        {
            var res = ForgeSchemaValidator.ValidateMultipleSchemasInPath(invalidSchemaDirectoryPath, jschemaRules,out List<string> errorList);
            Assert.AreEqual(false, res);
            Assert.AreEqual("The directory name is invalid.\r\n", errorList.First());
        }
        [TestMethod]
        public void Test_ValidateSchemaWithErrorContent()
        {
            var res = ForgeSchemaValidator.ValidateSchemaString(invalidSchemaWithErrorContent, jschemaRules, out List<string> errorList);
            Assert.AreEqual(false, res);
            Assert.AreEqual("JSON is valid against no schemas from 'oneOf'. line: 3, position: 17", errorList.First());
        }
        [TestMethod]
        public void Test_GetLinkedJSchemaRules()
        {
            var linkedRules = ForgeSchemaValidator.GetLinkedJSchemaRules(stringRules, stringRules, "//ForgeSchemaValidationRules.json", out string error);
            Assert.AreEqual("", error);
            ForgeSchemaValidator.ValidateSchema(treeSchema, linkedRules, out List<string> errorList);
            Assert.AreEqual(0, errorList.Count);
        }
    }
}
