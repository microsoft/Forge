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
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Forge.DataContracts;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Schema;
    using Microsoft.Forge.TreeWalker.UnitTests.test;
    using System.Threading.Tasks;

    [TestClass]
    public class TreeSchemaValidatorTests
    {
        private string stringSchema;
        private string stringSchemaList;
        private ForgeTree treeSchema;
        private List<ForgeTree> treeSchemaList;
        private string schemaPath;
        private string schemaDirectoryPath;
        private string stringRules;
        private JSchema jschemaRules;
        private ITreeSchemaValidator validator;
        private string invalidSchema;
        private string invalidSchemaDirectoryPath;

        [TestInitialize]
        public void TestInitialize()
        {
            validator = new TreeSchemaValidator();
            schemaPath = Path.Combine(Environment.CurrentDirectory, "test\\ExampleSchemas\\TardigradeSchema.json");
            schemaDirectoryPath = Path.Combine(Environment.CurrentDirectory, "test\\ExampleSchemas");
            stringRules = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "contracts\\ForgeSchemaValidationRules.json"));
            jschemaRules = JSchema.Parse(stringRules);
            stringSchema = File.ReadAllText(schemaPath);
            stringSchemaList = "{ Tree1:" + stringSchema + ", Tree2:" + stringSchema + "}";
            treeSchema =  JsonConvert.DeserializeObject<ForgeTree>((string)stringSchema);
            treeSchemaList = new List<ForgeTree> { treeSchema, treeSchema };
            invalidSchema = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "test\\InvalidTestSchemas\\InvalidTestSchema.json"));
            invalidSchemaDirectoryPath = "test\\ExampleSchemas\\TardigradeSchema.json";
        }
        [TestMethod]
        public async Task Test_ValidateSchemaInForgeTreeWithDefaultRulesAsync()
        {
            var res = await validator.ValidateSchema(treeSchema);
            Assert.AreEqual(true, res.Item1);
            Assert.AreEqual(0, res.Item2.Count);
        }
        [TestMethod]
        public async Task Test_ValidateSchemaInForgeTreeWithCustomRulesInStringAsync()
        {
            var res = await validator.ValidateSchema(treeSchema, stringRules);
            Assert.AreEqual(true, res.Item1);
            Assert.AreEqual(0, res.Item2.Count);
        }
        [TestMethod]
        public async Task Test_ValidateSchemaInForgeTreeWithCustomRulesInJSchemaAsync()
        {
            var res = await validator.ValidateSchema(treeSchema, jschemaRules);
            Assert.AreEqual(true, res.Item1);
            Assert.AreEqual(0, res.Item2.Count);
        }
        [TestMethod]
        public async Task Test_ValidateSchemaInForgeTreeListWithDefaultRulesAsync()
        {
            var res = await validator.ValidateSchemas(treeSchemaList);
            Assert.AreEqual(true, res.Item1);
            Assert.AreEqual(0, res.Item2.Count);
        }
        [TestMethod]
        public async Task Test_ValidateSchemaInForgeTreeListWithCustomRulesInStringAsync()
        {
            var res = await validator.ValidateSchema(treeSchema, stringRules);
            Assert.AreEqual(true, res.Item1);
            Assert.AreEqual(0, res.Item2.Count);
        }
        [TestMethod]
        public async Task Test_ValidateSchemaInForgeTreeListWithCustomRulesInJSchemaAsync()
        {
            var res = await validator.ValidateSchema(treeSchema, jschemaRules);
            Assert.AreEqual(true, res.Item1);
            Assert.AreEqual(0, res.Item2.Count);
        }
        [TestMethod]
        public async Task Test_ValidateSchemaInStringWithDefaultRulesAsync()
        {
            var res = await validator.ValidateSchemaString(stringSchema);
            Assert.AreEqual(true, res.Item1);
            Assert.AreEqual(0, res.Item2.Count);
        }
        [TestMethod]
        public async Task Test_ValidateSchemaInStringWithCustomRulesInStringAsync()
        {
            var res = await validator.ValidateSchemaString(stringSchema, stringRules);
            Assert.AreEqual(true, res.Item1);
            Assert.AreEqual(0, res.Item2.Count);
        }
        [TestMethod]
        public async Task Test_ValidateSchemaInStringWithCustomRulesInJSchemaAsync()
        {
            var res = await validator.ValidateSchemaString(stringSchema, jschemaRules);
            Assert.AreEqual(true, res.Item1);
            Assert.AreEqual(0, res.Item2.Count);
        }
        [TestMethod]
        public async Task Test_ValidateSchemasInStringListWithDefaultRulesAsync()
        {
            var res = await validator.ValidateSchemasString(stringSchemaList);
            Assert.AreEqual(true, res.Item1);
            Assert.AreEqual(0, res.Item2.Count);
        }
        [TestMethod]
        public async Task Test_ValidateSchemasInStringListWithCustomRulesInStringAsync()
        {
            var res = await validator.ValidateSchemasString(stringSchemaList, stringRules);
            Assert.AreEqual(true, res.Item1);
            Assert.AreEqual(0, res.Item2.Count);
        }
        [TestMethod]
        public async Task Test_ValidateSchemasInStringListWithCustomRulesInJSchemaAsync()
        {
            var res = await validator.ValidateSchemasString(stringSchemaList, jschemaRules);
            Assert.AreEqual(true, res.Item1);
            Assert.AreEqual(0, res.Item2.Count);
        }
        [TestMethod]
        public async Task Test_ValidateSchemaFromPathWithDefaultRulesAsync()
        {
            var res = await validator.ValidateSchemaInPath(schemaPath);
            Assert.AreEqual(true, res.Item1);
            Assert.AreEqual(0, res.Item2.Count);
        }
        [TestMethod]
        public async Task Test_ValidateSchemaFromPathWithCustomRulesInStringAsync()
        {
            var res = await validator.ValidateSchemaInPath(schemaPath, stringRules);
            Assert.AreEqual(true, res.Item1);
            Assert.AreEqual(0, res.Item2.Count);
        }
        [TestMethod]
        public async Task Test_ValidateSchemaFromPathWithCustomRulesInJSchemaAsync()
        {
            var res = await validator.ValidateSchemaInPath(schemaPath, jschemaRules);
            Assert.AreEqual(true, res.Item1);
            Assert.AreEqual(0, res.Item2.Count);
        }
        [TestMethod]
        public async Task Test_ValidateSchemasFromDirectoryListWithDefaultRulesAsync()
        {
            var res = await validator.ValidateMultipleSchemasInPath(schemaDirectoryPath);
            Assert.AreEqual(true, res.Item1);
            Assert.AreEqual(0, res.Item2.Count);
        }
        [TestMethod]
        public async Task Test_ValidateSchemasFromDirectoryListWithCustomRulesInStringAsync()
        {
            var res = await validator.ValidateMultipleSchemasInPath(schemaDirectoryPath, stringRules);
            Assert.AreEqual(true, res.Item1);
            Assert.AreEqual(0, res.Item2.Count);
        }
        [TestMethod]
        public async Task Test_ValidateSchemasFromDirectoryListWithCustomRulesInJSchemaAsync()
        {
            var res = await validator.ValidateMultipleSchemasInPath(schemaDirectoryPath, jschemaRules);
            Assert.AreEqual(true, res.Item1); 
            Assert.AreEqual(0, res.Item2.Count);
        }
        [TestMethod]
        public async Task Test_ValidateInvalidSchemaInStringAsync()
        {
            var res = await validator.ValidateSchemaString(invalidSchema);
            Assert.AreEqual(false, res.Item1);
            Assert.AreEqual("Required property 'Type' not found in JSON. Path 'Tree.Root', line 5, position 9.", res.Item2.First());
        }
        [TestMethod]
        public async Task Test_ValidateInvalidSchemaDirectoryPathAsync()
        {
            var res = await validator.ValidateMultipleSchemasInPath(invalidSchemaDirectoryPath);
            Assert.AreEqual(false, res.Item1);
            Assert.AreEqual("Can not get schema in string from files in the given directory", res.Item2.First());
        }
    }
}
