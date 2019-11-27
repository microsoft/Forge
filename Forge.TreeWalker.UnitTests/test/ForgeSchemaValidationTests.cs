//-----------------------------------------------------------------------
// <copyright file="ForgeSchemaValidationTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Tests the ForgeSchemaValidationRules.json file against schemas in ForgeSchemaHelper class.
// </summary>
//-----------------------------------------------------------------------

namespace Forge.TreeWalker.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Forge.DataContracts;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Schema;

    [TestClass]
    public class ForgeSchemaValidationTests
    {
        private string jsonSchemaRules;
        private JSchema rules;
        private Dictionary<string, List<string>> jsonSchemaFailureBlacklist;

        [TestInitialize]
        public void TestInitialize()
        {
            // Load the json schema validation rules.
            this.jsonSchemaRules = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "contracts\\ForgeSchemaValidationRules.json"));
            this.rules = JSchema.Parse(this.jsonSchemaRules);

            // Update the blacklist of forgeTrees that are expected to fail validation for testing purposes.
            // Key is the const string variable name representing a schema file in ForgeSchemaHelper.
            // Value is the list of TreeNames in the schema if the schema has TreeNames. Or "NA" if the schema contains just a ForgeTree without TreeName.
            this.jsonSchemaFailureBlacklist = new Dictionary<string, List<string>>()
            {
                {
                    "SubroutineAction_FailsOnActionTreeNodeType",
                    new List<string>
                    {
                        "RootTree"
                    }
                },
                {
                    "SubroutineAction_FailsOnNoSubroutineAction",
                    new List<string>
                    {
                        "RootTree"
                    }
                }
            };
        }

        [TestMethod]
        public void Test_ValidationRulesAreValid()
        {
            // Get all the const string schemas from ForgeSchemaHelper.
            List<FieldInfo> schemas = this.GetAllPublicConstantFields(typeof(ForgeSchemaHelper));
            Console.WriteLine("Count of unique schemas getting evaluated: " + schemas.Count);
            int treeCount = 0;
            
            // Iterate through each schema to run validations.
            foreach (FieldInfo fieldInfo in schemas)
            {
                Console.WriteLine(fieldInfo.Name);
                string jsonSchema = (string)fieldInfo.GetRawConstantValue();
                List<Tuple<string, bool>> jsonTrees = new List<Tuple<string, bool>>();

                // jsonSchema may be deserialized to either a Dictionary<string, ForgeTree> (containing multiple trees), or a single ForgeTree.
                // Gather all the individual ForgeTree(s) from the schema and cache them in jsonTrees with their expectedResult.
                try
                {
                    Dictionary<string, ForgeTree> forgeTrees = JsonConvert.DeserializeObject<Dictionary<string, ForgeTree>>(jsonSchema);
                    
                    foreach (var kvp in forgeTrees)
                    {
                        string treeName = kvp.Key;
                        ForgeTree forgeTree = kvp.Value;

                        if (forgeTree.Tree == null)
                        {
                            // Deserialize into Dictionary does not throw exception but will have null "Tree" property if schema is just a ForgeTree.
                            // Throw exception here to trigger deserializing into ForgeTree directly.
                            throw new NullReferenceException();
                        }

                        string jsonSubSchema = JsonConvert.SerializeObject(
                            forgeTree,
                            new JsonSerializerSettings
                            {
                                DefaultValueHandling = DefaultValueHandling.Ignore, // Prevent default values from getting added to serialized json schema.
                                Converters = new List<JsonConverter> { new Newtonsoft.Json.Converters.StringEnumConverter() } // Use string enum values instead of numerical.
                            });

                        // expectedResult is false if this schema/TreeName is blacklisted.
                        bool expectedResult = !(this.jsonSchemaFailureBlacklist.TryGetValue(fieldInfo.Name, out List<string> list) && list.Contains(treeName));
                        jsonTrees.Add(new Tuple<string, bool>(jsonSubSchema, expectedResult));
                    }
                    Console.WriteLine("DICTIONARY NO THROW");
                }
                catch (Exception)
                {
                    try
                    {
                        // Verify that schema can be deserialized.
                        JsonConvert.DeserializeObject<ForgeTree>(jsonSchema);

                        // expectedResult is false if this schema/TreeName is blacklisted.
                        bool expectedResult = !(this.jsonSchemaFailureBlacklist.TryGetValue(fieldInfo.Name, out List<string> list) && list.Contains("NA"));
                        jsonTrees.Add(new Tuple<string, bool>(jsonSchema, expectedResult));
                    }
                    catch (Exception)
                    {
                        Assert.Fail("ForgeSchema for property (" + fieldInfo.Name + ") did not deserialize to a ForgeTree or Dictionary<string, ForgeTree>.");
                    }
                }

                // Validate each ForgeTree in this schema according to their expectedResult.
                foreach (var tuple in jsonTrees)
                {
                    treeCount++;
                    string schema = tuple.Item1;
                    bool expectedResult = tuple.Item2;
                    this.Validate(jsonSchema: schema, expectedResult: expectedResult);
                }
            }

            Console.WriteLine("Count of unique ForgeTrees getting evaluated: " + treeCount);
        }

        private void Validate(string jsonSchema, bool expectedResult)
        {
            JObject schema = JObject.Parse(jsonSchema);

            // Check if the forgeTree schema is valid against the json schema validation rules.
            bool isValid = schema.IsValid(this.rules, out IList<ValidationError> errors);
            Console.WriteLine("IsValid: " + isValid + ", ExpectedResult: " + expectedResult);

            foreach (var error in errors)
            {
                this.PrintValidationErrors(error);
                Console.WriteLine(jsonSchema);
            }

            if (expectedResult)
            {
                Assert.IsTrue(isValid);
            }
            else
            {
                Assert.IsFalse(isValid);
            }
        }

        private void PrintValidationErrors (ValidationError error)
        {
            Console.WriteLine(error.Message);
            Console.WriteLine(error.Path);
            Console.WriteLine(error.ErrorType);
            Console.WriteLine(error.SchemaBaseUri);
            Console.WriteLine(error.SchemaId);
            Console.WriteLine(error.Schema);

            foreach (var child in error.ChildErrors)
            {
                this.PrintValidationErrors(child);
            }
        }

        private List<FieldInfo> GetAllPublicConstantFields(Type type)
        {
            return type
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .ToList();
        }
    }
}
