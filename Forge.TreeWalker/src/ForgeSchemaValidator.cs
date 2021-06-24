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
        public static bool ValidateSchema(ForgeTree schema, object rules, out List<string> errorList)
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
        public static bool ValidateSchemas(Dictionary<string, ForgeTree> schemas, object rules, bool validateAsDictionary, out List<string> errorList)
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
        public static bool ValidateSchemaString(string schema, object rules, bool validateAsDictionary, out List<string> errorList)
        {
            List<JObject> schemaList = ConvertStringToJObjectList(schema, out errorList, validateAsDictionary);

            return CheckConvertErrorAndValidate(rules, ref errorList, schemaList);
        }

        /// <summary>
        /// Validates single or multiple schemas from input path with the given rules.        
        /// </summary>
        /// <param name="path">The path that contains a schema file</param>
        /// <param name="rules">The rules used to validate input schemas is only allowed in string or JSchema type</param>
        /// <param name="validateAsDictionary">True if the custom rules is to handle the whole dictionary</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        public static bool ValidateSchemaInPath(string path, object rules, bool validateAsDictionary, out List<string> errorList)
        {
            List<JObject> schemas = GetSchemaFromPath(path, out errorList, validateAsDictionary);

            return CheckConvertErrorAndValidate(rules, ref errorList, schemas);
        }

        /// <summary>
        /// Validates single or multiple schemas from input path with the given rules.
        /// </summary>
        /// <param name="path">The path that contains a schemas directory</param>
        /// <param name="rules">The rules used to validate input schemas is only allowed in string or JSchema type</param>
        /// <param name="validateAsDictionary">True if the custom rules is to handle the whole dictionary</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        public static bool ValidateMultipleSchemasInPath(string path, object rules, bool validateAsDictionary, out List<string> errorList)
        {
            List<JObject> schemas = GetAllSchemasInDirectory(path, out errorList, validateAsDictionary);

            return CheckConvertErrorAndValidate(rules, ref errorList, schemas);
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

        private static List<JObject> ConvertStringToJObjectList(string schema, out List<string> errorList, bool validateAsDictionary)
        {
            List<JObject> schemaList = new List<JObject>();
            errorList = new List<string>();

            try
            {
                //There could be three possible cases:
                //1. TreeName mapped to the ForgeTree in the dictionary and custom rules handle dictionary.
                //2. There are only ForgeTree without matching forge tree name custom rules handle ForgeTree list.
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
            }
            catch (Exception e)
            {
                errorList.Add(e.Message);
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

        private static List<JObject> GetSchemaFromPath(string path, out List<string> errorList, bool validateAsDictionary)
        {
            errorList = new List<string>();

            try
            {
                string schema = File.ReadAllText(path);

                List<JObject> res = ConvertStringToJObjectList(schema, out List<string> convertError, validateAsDictionary);
                errorList = convertError;

                return res;
            }
            catch (Exception e)
            {
                errorList.Add(e.Message);

                return new List<JObject>();
            }
        }

        private static List<JObject> GetAllSchemasInDirectory(string path, out List<string> errorList, bool validateAsDictionary)
        {
            List<JObject> schemaList = new List<JObject>();
            errorList = new List<string>();

            try
            {
                string[] Files = Directory.GetFiles(path);
                List<object> schemalist = new List<object>();

                if (validateAsDictionary) {
                    Dictionary<string, ForgeTree> combinedDictionary = new Dictionary<string, ForgeTree>();
                    
                    foreach (string file in Files) 
                    {
                        string schema = File.ReadAllText(file);
                        Dictionary<string, ForgeTree> schemaDictionary = JsonConvert.DeserializeObject<Dictionary<string, ForgeTree>>(schema);
                        
                        foreach (KeyValuePair<string, ForgeTree> item in schemaDictionary) 
                        {
                            combinedDictionary.Add(item.Key, item.Value);
                        }
                    }

                    return new List<JObject> { SerializeToJObject(combinedDictionary) };
                }
                else
                {
                    foreach (string file in Files)
                    {
                        List<JObject> schemasInFile = GetSchemaFromPath(file, out errorList, validateAsDictionary);

                        if (errorList.Count > 0)
                        {
                            break;
                        }

                        schemasInFile.ForEach(n => schemaList.Add(n));
                    }
                }
            }
            catch (Exception e)
            {
                errorList.Add(e.Message);
            }

            return schemaList;
        }

        private static bool CheckConvertErrorAndValidate(Object rules, ref List<string> errorList, List<JObject> schemaList)
        {
            if (errorList.Count > 0)
            {
                return false;
            }

            return Validate(schemaList, rules, out errorList);
        }

        private static bool Validate(List<JObject> schemas, Object rules, out List<string> errorList)
        {
            errorList = new List<string>();
            JSchema jSchemaRules = null;

            if (rules is string)
            {
                jSchemaRules = JSchema.Parse((string)rules);
            }
            else if (rules is JSchema)
            {
                jSchemaRules = (JSchema)rules;
            }
            else 
            {
                errorList.Add("Rules type could only be string or JSchema");

                return false;
            }

            if (schemas.Count == 0)
            {
                errorList.Add("Can't find target schema to test or file type is not supported");

                return false;
            }

            foreach (JObject schema in schemas)
            {
                if (!schema.IsValid(jSchemaRules, out IList<ValidationError> errorDetail))
                {
                    foreach (ValidationError error in errorDetail)
                    {
                        errorList.Add(error.Message + " line: " + error.LineNumber + ", position: " + error.LinePosition);
                    }

                    return false;
                }
            }

            return true;
        }
    }
}
