//-----------------------------------------------------------------------
// <copyright file="ForgeSchemaValidator.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The ForgeSchemaValidator class.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.TreeWalker
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Forge.DataContracts;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Schema;

    /// <summary>
    /// The ForgeSchemaValidator class implements the validation method that tests input schemas with input custom rules.
    /// </summary>
    public static class ForgeSchemaValidator
    {

        /// <summary>
        /// Linked rules in JSchema type.
        /// </summary>
        /// <param name="childRules">The rules to be included in the parent rules</param>
        /// <param name="parentRules">The parent rules to absorb the child rules</param>
        /// <param name="referenceUri">The address of childRules</param>
        /// <returns>The result of schema combination.</returns>
        public static JSchema GetLinkedJSchemaRules(string childRules, string parentRules, string referenceUri)
        {
            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
            resolver.Add(new Uri(referenceUri), childRules);

            return JSchema.Parse(parentRules, resolver);
        }

        /// <summary>
        /// Validates the ForgeTree schema with the given rules.
        /// </summary>
        /// <param name="schema">The schema to be validated</param>
        /// <param name="rules">The rules used to validate input schemas is only allowed in string or JSchema type</param>
        /// /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        public static bool ValidateSchemaAsForgeTree(ForgeTree schema, object rules, out IList<ValidationError> errorList)
        {
            return Validate(new List<JObject> { SerializeToJObject(schema) }, rules, out errorList);
        }

        /// <summary>
        /// Validates single or multiple schemas in Dictionary with the given rules.
        /// </summary>
        /// <param name="schemas">The schemas to be validated</param>
        /// <param name="rules">The rules used to validate input schemas is only allowed in string or JSchema type</param>
        /// <param name="validateAsDictionary">True if the custom rules is to handle the whole dictionary</param>        
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        public static bool ValidateSchemaAsForgeTreeDictionary(Dictionary<string, ForgeTree> schemas, object rules, bool validateAsDictionary, out IList<ValidationError> errorList)
        {
            return Validate(ConvertDictionaryToJObjectList(schemas, validateAsDictionary), rules, out errorList);
        }

        /// <summary>
        /// Validates single or multiple schemas in string with the given rules.
        /// </summary>
        /// <param name="schema">The schema to be validated</param>
        /// <param name="rules">The rules used to validate input schemas is only allowed in string or JSchema type</param>
        /// <param name="validateAsDictionary">True if the custom rules is to handle the whole dictionary</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        public static bool ValidateSchemaAsString(string schema, object rules, bool validateAsDictionary, out IList<ValidationError> errorList)
        {
            List<JObject> schemaList = ConvertStringToJObjectList(schema, validateAsDictionary);

            return Validate(schemaList, rules, out errorList);
        }

        /// <summary>
        /// Validates single or multiple schemas from input path with the given rules.        
        /// </summary>
        /// <param name="path">The path that contains a schema file</param>
        /// <param name="rules">The rules used to validate input schemas is only allowed in string or JSchema type</param>
        /// <param name="validateAsDictionary">True if the custom rules is to handle the whole dictionary</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        public static bool ValidateSchemaFromPath(string path, object rules, bool validateAsDictionary, out IList<ValidationError> errorList)
        {
            List<JObject> schemas = GetSchemaFromPath(path, validateAsDictionary);

            return Validate(schemas, rules, out errorList);
        }

        /// <summary>
        /// Validates single or multiple schemas from input path with the given rules.
        /// </summary>
        /// <param name="path">The path that contains a schemas directory</param>
        /// <param name="rules">The rules used to validate input schemas is only allowed in string or JSchema type</param>
        /// <param name="validateAsDictionary">True if the custom rules is to handle the whole dictionary</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        public static bool ValidateSchemaFromDirectory(string path, object rules, bool validateAsDictionary, out IList<ValidationError> errorList)
        {
            List<JObject> schemas = GetAllSchemasInDirectory(path, validateAsDictionary);

            return Validate(schemas, rules, out errorList);
        }


        private static List<JObject> ConvertDictionaryToJObjectList(Dictionary<string, ForgeTree> schemas, bool validateAsDictionary)
        {
            List<JObject> schemaList = new List<JObject>();

            if (validateAsDictionary)
            {
                schemaList.Add(SerializeToJObject(schemas));
            }
            else
            {
                foreach (ForgeTree item in schemas.Values)
                {
                    schemaList.Add(SerializeToJObject(item));
                }
            }

            return schemaList;
        }

        private static List<JObject> ConvertStringToJObjectList(string schema, bool validateAsDictionary)
        {
            List<JObject> schemaList = new List<JObject>();

            //There could be three possible cases:
            //1. TreeName mapped to the ForgeTree in the dictionary and custom rules handle dictionary.
            //2. There are only ForgeTree without matching forge tree name custom rules handle ForgeTree list.
            //3. The string schema that could be deserialized as a ForgeTree should be parsed directly
            Dictionary<string, ForgeTree> forgeTrees = JsonConvert.DeserializeObject<Dictionary<string, ForgeTree>>(schema);

            if (validateAsDictionary)
            {
                schemaList.Add(JObject.Parse(schema));
            }
            else
            {
                foreach (KeyValuePair<string, ForgeTree> kvp in forgeTrees)
                {
                    ForgeTree forgeTree = kvp.Value;

                    if (forgeTree.Tree == null)
                    {
                        // Deserialize into Dictionary does not throw exception but will have null "Tree" property if schema is just a ForgeTree.
                        // try to deserialize string to forge tree directly
                        JsonConvert.DeserializeObject<ForgeTree>(schema);
                        schemaList.Add(JObject.Parse(schema));
                        break;
                    }

                    schemaList.Add(SerializeToJObject(forgeTree));
                }
            }

            return schemaList;
        }

        private static JObject SerializeToJObject(object forgeTree)
        {
            string stringSchema = JsonConvert.SerializeObject(
                forgeTree,
                new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore, // Prevent default values from getting added to serialized json schema.
                    Converters = new List<JsonConverter> { new Newtonsoft.Json.Converters.StringEnumConverter() } // Use string enum values instead of numerical.
                });

            return JObject.Parse(stringSchema);
        }

        private static List<JObject> GetSchemaFromPath(string path, bool validateAsDictionary)
        {
            string schema = File.ReadAllText(path);
            List<JObject> res = ConvertStringToJObjectList(schema, validateAsDictionary);

            return res;
        }

        private static List<JObject> GetAllSchemasInDirectory(string path, bool validateAsDictionary)
        {
            List<JObject> schemaList = new List<JObject>();
            string[] files = Directory.GetFiles(path);
            List<object> schemalist = new List<object>();

            if (validateAsDictionary)
            {
                Dictionary<string, ForgeTree> combinedDictionary = new Dictionary<string, ForgeTree>();

                foreach (string file in files)
                {
                    string schema = File.ReadAllText(file);
                    Dictionary<string, ForgeTree> schemaDictionary = JsonConvert.DeserializeObject<Dictionary<string, ForgeTree>>(schema);

                    foreach (KeyValuePair<string, ForgeTree> kvp in schemaDictionary)
                    {
                        combinedDictionary.Add(kvp.Key, kvp.Value);
                    }
                }

                return new List<JObject> { SerializeToJObject(combinedDictionary) };
            }
            else
            {
                foreach (string file in files)
                {
                    List<JObject> schemasInFile = GetSchemaFromPath(file, validateAsDictionary);
                    schemasInFile.ForEach(n => schemaList.Add(n));
                }
            }

            return schemaList;
        }

        /// <summary>
        /// Validate the input Schemas in JObject against rules in JSchema and return the validation error list. 
        /// </summary>
        /// <param name="schemas">One or serveral schemas to be validated. The number of JObjects depends on the input and three cases mentioned in ConvertStringToJObjectList method</param>
        /// <param name="rules">The rules used to validate input schemas is only allowed in string or JSchema type</param>
        /// <returns>The result of schema validation. The errorList is the origin error message from IsValid method</returns>
        private static bool Validate(List<JObject> schemas, object rules, out IList<ValidationError> errorList)
        {
            JSchema jSchemaRules = new JSchema();
            errorList = new List<ValidationError>();

            if (rules is string)
            {
                jSchemaRules = JSchema.Parse((string)rules);
            }
            else if (rules is JSchema)
            {
                jSchemaRules = (JSchema)rules;
            }

            if (schemas.Count == 0)
            {
                return false;
            }

            foreach (JObject schema in schemas)
            {
                if (!schema.IsValid(jSchemaRules, out errorList))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
