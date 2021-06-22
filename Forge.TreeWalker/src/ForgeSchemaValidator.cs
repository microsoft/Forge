//-----------------------------------------------------------------------
// <copyright file="ForgeSchemaValidator.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The ForgeSchemaValidator class implements the ITreeSchemaValidator interface.
// </summary>
//-----------------------------------------------------------------------
namespace Microsoft.Forge.TreeWalker
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Forge.DataContracts;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Schema;
    /// <summary>
    /// The ForgeSchemaValidator class implements the validation method that tests input schemas with custom rules from input.
    /// </summary>
    public static class ForgeSchemaValidator
    {
        /// <summary>
        /// The GetLinkedJSchema method that creates the linked rules in JSchema.
        /// </summary>
        /// <param name="childRules">The rules to be included in the parent rules</param>
        /// <param name="parentRules">The parent rules to absorb the child rules</param>
        /// <param name="referenceUri">The address of childRules</param>
        /// <returns>The result of schema combination. ErrorMessage would be set if it throw exceptions</returns>
        public static JSchema GetLinkedJSchemaRules(string childRules, string parentRules, string referenceUri, out string errorMessage)
        {
            try
            {
                JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
                resolver.Add(new Uri(referenceUri), childRules);
                errorMessage = "";
                return JSchema.Parse(parentRules, resolver);
            }
            catch(Exception e)
            {
                errorMessage = e.Message;
                return null;
            }
        }

        /// <summary>
        /// The validate task that validate the input schema with custom rules in string.
        /// </summary>
        /// <param name="schema">The schema to be validated</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        public static bool ValidateSchema(ForgeTree schema, string rules, out List<string> errorList)
        {
            return Validate(new List<JObject> { SerializeForgeTree(schema) }, JSchema.Parse(rules), out errorList);
        }
        /// <summary>
        /// The validate task that validate the input schema with custom rules in string.
        /// </summary>
        /// <param name="schema">The schema to be validated</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        public static bool ValidateSchema(ForgeTree schema, JSchema rules, out List<string> errorList)
        {
            return Validate(new List<JObject> { SerializeForgeTree(schema) }, rules, out errorList);
        }
        /// <summary>
        /// The validate task that validate multiple input schemas with custom rules in string.
        /// </summary>
        /// <param name="schemas">The schemas to be validated</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        public static bool ValidateSchemas(Dictionary<string, ForgeTree> schemas, string rules, out List<string> errorList)
        {
            return Validate(ConvertDictionaryToForgeTreeList(schemas), JSchema.Parse(rules), out errorList);
        }
        /// <summary>
        /// The validate task that check the input schema in string with custom rules in string.
        /// </summary>
        /// <param name="schema">The schema to be validated</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        public static bool ValidateSchemaString(string schema, string rules, out List<string> errorList)
        {
            var schemaList = ConvertStringToJObjectList(schema, out errorList);
            return CheckConvertErrorAndValidate(JSchema.Parse(rules), ref errorList, schemaList);
        }

        /// <summary>
        /// The validate task that validate the schema in the input file path with custom rules in string.
        /// </summary>
        /// <param name="path">The path that contains a schema file</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        public static bool ValidateSchemaInPath(string path, string rules, out List<string> errorList)
        {
            var schemas = GetSchemaFromPath(path, out errorList);
            return CheckConvertErrorAndValidate(JSchema.Parse(rules), ref errorList, schemas);
        }

        /// <summary>
        /// The validate task that validate all schemas in a directory with custom rules in string.
        /// </summary>
        /// <param name="path">The path that contains a schemas directory</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        public static bool ValidateMultipleSchemasInPath(string path, string rules, out List<string> errorList)
        {
            var schemas = GetAllSchemasInDirectory(path, out errorList);
            return CheckConvertErrorAndValidate(JSchema.Parse(rules), ref errorList, schemas);
        }
        /// <summary>
        /// The validate task that validate multiple input schemas with custom rules in JSchema.
        /// </summary>
        /// <param name="schemas">The schemas to be validated</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        public static bool ValidateSchemas(Dictionary<string, ForgeTree> schemas, JSchema rules, out List<string> errorList)
        {
            return Validate(ConvertDictionaryToForgeTreeList(schemas), rules, out errorList);
        }
        /// <summary>
        /// The validate task that validate the schema in the input file path with custom rules in JSchema.
        /// </summary>
        /// <param name="path">The path that contains a schema file</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        public static bool ValidateSchemaInPath(string path, JSchema rules, out List<string> errorList)
        {
            var schema = GetSchemaFromPath(path, out errorList);
            return CheckConvertErrorAndValidate(rules, ref errorList, schema);
        }
        /// <summary>
        /// The validate task that validate all schemas in a directory with custom rules in JSchema.
        /// </summary>
        /// <param name="path">The path that contains a schemas directory</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        public static bool ValidateMultipleSchemasInPath(string path, JSchema rules, out List<string> errorList)
        {
            var schemas = GetAllSchemasInDirectory(path, out errorList);
            return CheckConvertErrorAndValidate(rules, ref errorList, schemas);
        }

        /// <summary>
        /// The validate task that check the input schema in string with custom rules in JSchema.
        /// </summary>
        /// <param name="schema">The schema to be validated</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        public static bool ValidateSchemaString(string schema, JSchema rules, out List<string> errorList)
        {
            var schemaList = ConvertStringToJObjectList(schema, out errorList);
            if (errorList.Count > 0)
            {
                return false;
            }
            var res = Validate(schemaList, rules, out errorList); 
            return res;
        }

        private static List<JObject> ConvertDictionaryToForgeTreeList(Dictionary<string, ForgeTree> schemas)
        {
            var schemaList = new List<JObject>();
            foreach (var item in schemas.Values)
                schemaList.Add(SerializeForgeTree(item));
            return schemaList;
        }

        private static List<JObject> ConvertStringToJObjectList(string schema, out List<string> errorList)
        {
            var schemaList = new List<JObject>();
            errorList = new List<string>();
            try
            {
                Dictionary<string, ForgeTree> forgeTrees = JsonConvert.DeserializeObject<Dictionary<string, ForgeTree>>(schema);
                foreach (var kvp in forgeTrees)
                {
                    ForgeTree forgeTree = kvp.Value;
                    if (forgeTree.Tree == null)
                    {
                        // Deserialize into Dictionary does not throw exception but will have null "Tree" property if schema is just a ForgeTree.
                        // try to deserialize string to forge tree directly
                        JsonConvert.DeserializeObject<ForgeTree>(schema);
                        JObject res = JObject.Parse(schema);
                        schemaList.Add(res);
                        break;
                    }
                    schemaList.Add(SerializeForgeTree(forgeTree));
                }
            }
            catch (Exception e)
            {
                errorList.Add(e.Message);
            }
            return schemaList;
        }

        private static JObject SerializeForgeTree(ForgeTree forgeTree)
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

        private static List<JObject> GetSchemaFromPath(string path, out List<string> errorList)
        {
            errorList = new List<string>();
            try
            {
                var schema = File.ReadAllText(path);
                var res = ConvertStringToJObjectList(schema, out List<string> convertError);
                errorList = convertError;
                return res;
            }
            catch (Exception e)
            {
                errorList.Add(e.Message);
                return new List<JObject>();
            }
        }

        private static List<JObject> GetAllSchemasInDirectory(string path, out List<string> errorList)
        {
            var schemaList = new List<JObject>();
            errorList = new List<string>();
            try
            {
                string[] Files = Directory.GetFiles(path);
                var schemalist = new List<object>();
                foreach (string file in Files)
                {
                    var schemasInFile = GetSchemaFromPath(file, out errorList);
                    if (errorList.Count > 0)
                    {
                        break;
                    }
                    schemasInFile.ForEach(n => schemaList.Add(n));
                }
            }
            catch (Exception e)
            {
                errorList.Add(e.Message);
            }
            return schemaList;
        }

        private static bool CheckConvertErrorAndValidate(JSchema rules, ref List<string> errorList, List<JObject> schemaList)
        {
            if (errorList.Count > 0)
            {
                return false;
            }
            return Validate(schemaList, rules, out errorList);
        }

        private static bool Validate(List<JObject> schemas, JSchema rules, out List<string> errorList)
        {
            errorList = new List<string>();
            if (schemas.Count == 0)
            {
                errorList.Add("Can't find target schema to test or file type is not supported");
                return false;
            }
            foreach (var schema in schemas)
            {
                if (!schema.IsValid(rules, out IList<ValidationError> errorDetail))
                {
                    foreach (var error in errorDetail)
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
