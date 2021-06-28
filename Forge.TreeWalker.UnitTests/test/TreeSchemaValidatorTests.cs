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
        private string tardigradeSchemaString;
        private ForgeTree treeSchema;
        private Dictionary<string, ForgeTree> treeSchemas;
        private string tardigradeSchemaPath;
        private string schemaDirectoryPath;
        private string stringRules;
        private string rulesForDictionary;
        private JSchema jschemaRules;
        private string invalidSchemaDirectoryPath;
        private string invalidSchemaWithErrorContent;
        private string subroutineSchemaString;

        [TestInitialize]
        public void TestInitialize()
        {
            tardigradeSchemaPath = Path.Combine(Environment.CurrentDirectory, "test\\ExampleSchemas\\TardigradeSchema.json");
            schemaDirectoryPath = Path.Combine(Environment.CurrentDirectory, "test\\ExampleSchemas");
            stringRules = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "contracts\\ForgeSchemaValidationRules.json"));
            rulesForDictionary = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "contracts\\ForgeSchemaDictionaryValidationRules.json"));
            jschemaRules = JSchema.Parse(stringRules);
            tardigradeSchemaString = File.ReadAllText(tardigradeSchemaPath);
            subroutineSchemaString = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "test\\ExampleSchemas\\SubroutineSchema.json"));
            treeSchema =  JsonConvert.DeserializeObject<ForgeTree>((string)tardigradeSchemaString);
            treeSchemas = new Dictionary<string, ForgeTree>();
            treeSchemas.Add("tree1", treeSchema);
            treeSchemas.Add("tree2", treeSchema);
            treeSchemas.Add("tree3", treeSchema);
            invalidSchemaWithErrorContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "test\\InvalidTestSchemas\\InvalidTestSchemaErrorContent.json"));
            invalidSchemaDirectoryPath = "test\\ExampleSchemas\\TardigradeSchema.json";
        }

        [TestMethod]
        public void Test_ValidateSchemaInForgeTreeWithCustomRulesInString()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsForgeTree(treeSchema, stringRules, out IList<ValidationError> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaInForgeTreeWithCustomRulesInJSchema()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsForgeTree(treeSchema, jschemaRules, out IList<ValidationError> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaInForgeTreeDictionaryWithCustomRulesInString()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsForgeTreeDictionary(treeSchemas, stringRules, false, out IList<ValidationError> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaInForgeTreeDictionaryWithCustomRulesInJSchema()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsForgeTreeDictionary(treeSchemas, jschemaRules, false, out IList <ValidationError> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaInStringWithCustomRules()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsString(tardigradeSchemaString, stringRules, false, out IList<ValidationError> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaInStringWithCustomRulesInJSchema()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsString(tardigradeSchemaString, jschemaRules, false, out IList<ValidationError> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaFromPathWithCustomRulesInString()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaFromPath(tardigradeSchemaPath, stringRules, false, out IList<ValidationError> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemaFromPathWithCustomRulesInJSchema()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaFromPath(tardigradeSchemaPath, jschemaRules, false, out IList<ValidationError> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemasFromDirectoryListWithCustomRulesInString()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaFromDirectory(schemaDirectoryPath, stringRules, false, out IList<ValidationError> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateSchemasFromDirectoryListWithCustomRulesInJSchema()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaFromDirectory(schemaDirectoryPath, jschemaRules, false, out IList<ValidationError> errorList);
            Assert.AreEqual(true, res); 
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateInvalidSchemaInString()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsString(tardigradeSchemaString, jschemaRules, false, out IList<ValidationError> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateInvalidSchemaDirectoryPath()
        {
            try
            {
                ForgeSchemaValidator.ValidateSchemaFromDirectory(invalidSchemaDirectoryPath, jschemaRules, false, out IList<ValidationError> errorList);
            }
            catch (Exception e)
            {
                Assert.AreEqual("The directory name is invalid.\r\n", e.Message);
            }
        }

        [TestMethod]
        public void Test_ValidateSchemaWithErrorContent()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsString(invalidSchemaWithErrorContent, jschemaRules, false, out IList<ValidationError> errorList);
            Assert.AreEqual(false, res);
            Assert.AreEqual("JSON is valid against no schemas from 'oneOf'.", errorList.First().Message);
        }

        [TestMethod]
        public void Test_ValidateInvalidSchemaAsDictionary()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsString(invalidSchemaWithErrorContent, jschemaRules, true, out IList<ValidationError> errorList);
            Assert.AreEqual(false, res);
            Assert.AreEqual("JSON is valid against no schemas from 'oneOf'.", errorList.First().Message);
        }

        [TestMethod]
        public void Test_ValidateValidSchemaAsDictionary()
        {
            bool res = ForgeSchemaValidator.ValidateSchemaAsString(subroutineSchemaString, rulesForDictionary, true, out IList<ValidationError> errorList);
            Assert.AreEqual(true, res);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void Test_ValidateAsDictionaryContainsDuplicateKey()
        {
            try
            {
                ForgeSchemaValidator.ValidateSchemaFromDirectory(schemaDirectoryPath, stringRules, true, out IList<ValidationError> errorList);
            }
            catch (Exception e)
            {
                Assert.AreEqual("An item with the same key has already been added.", e.Message);
            }
        }

        [TestMethod]
        public void Test_GetLinkedJSchemaRules()
        {
            try
            {
                JSchema linkedRules = ForgeSchemaValidator.GetLinkedJSchemaRules(stringRules, stringRules, "//ForgeSchemaValidationRules.json");
                ForgeSchemaValidator.ValidateSchemaAsForgeTree(treeSchema, linkedRules, out IList < ValidationError > errorList);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }      
        }
    }
}
