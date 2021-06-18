//-----------------------------------------------------------------------
// <copyright file="TreeSchemaValidator.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The TreeSchemaValidator class implements the ITreeSchemaValidator interface.
// </summary>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Forge.DataContracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;


namespace Microsoft.Forge.TreeWalker
{
    public class TreeSchemaValidator : ITreeSchemaValidator
    {
        private JSchema ForgeSchemaValidationRules;
        public TreeSchemaValidator() => ForgeSchemaValidationRules = GetRulesFromPath("contracts\\ForgeSchemaValidationRules.json");

        public async Task<Tuple<bool, IList<string>>> ValidateSchema(ForgeTree schema)
        {
            return await Task.Run(() => Validate(new List<object> { schema }, ForgeSchemaValidationRules));
        }

        public async Task<Tuple<bool, IList<string>>> ValidateSchemas(IList<ForgeTree> schemas)
        {
            return await Task.Run(() => Validate(ConvertSchemasToSchemaList(schemas), ForgeSchemaValidationRules));
        }

        public async Task<Tuple<bool, IList<string>>> ValidateSchemaString(string schema)
        {
            return await Task.Run(() => Validate(new List<object> { schema }, ForgeSchemaValidationRules));
        }

        public async Task<Tuple<bool, IList<string>>> ValidateSchemasString(string schemas)
        {
            return await Task.Run(() => Validate(new List<object> { schemas }, ForgeSchemaValidationRules));
        }

        public async Task<Tuple<bool, IList<string>>> ValidateSchemaInPath(string path)
        {
            return await GetschemaFromPathAndValidate(path, ForgeSchemaValidationRules);
        }

        public async Task<Tuple<bool, IList<string>>> ValidateMultipleSchemasInPath(string path)
        {
            return await GetAllSchemasInDirectoryAndValidate(path, ForgeSchemaValidationRules);
        }

        public async Task<Tuple<bool, IList<string>>> ValidateSchema(ForgeTree schema, string rules)
        {
            return await Task.Run(() => Validate(new List<object> { schema }, rules));
        }

        public async Task<Tuple<bool, IList<string>>> ValidateSchemas(IList<ForgeTree> schemas, string rules)
        {
            return await Task.Run(() => Validate(ConvertSchemasToSchemaList(schemas), rules));
        }

        public async Task<Tuple<bool, IList<string>>> ValidateSchemaString(string schema, string rules)
        {
            return await Task.Run(() => Validate(new List<object> { schema }, rules));
        }

        public async Task<Tuple<bool, IList<string>>> ValidateSchemasString(string schemas, string rules)
        {
            return await Task.Run(() => Validate(new List<object> { schemas }, ForgeSchemaValidationRules));
        }

        public async Task<Tuple<bool, IList<string>>> ValidateSchemaInPath(string path, string rules)
        {
            return await GetschemaFromPathAndValidate(path, rules);
        }

        public async Task<Tuple<bool, IList<string>>> ValidateMultipleSchemasInPath(string path, string rules)
        {
            return await GetAllSchemasInDirectoryAndValidate(path, rules);
        }

        public async Task<Tuple<bool, IList<string>>> ValidateSchema(ForgeTree schema, JSchema rules)
        {
            return await Task.Run(() => Validate(new List<object> { schema }, ForgeSchemaValidationRules));
        }

        public async Task<Tuple<bool, IList<string>>> ValidateSchemas(IList<ForgeTree> schemas, JSchema rules)
        {
            return await Task.Run(() => Validate(ConvertSchemasToSchemaList(schemas), rules));
        }

        public async Task<Tuple<bool, IList<string>>> ValidateSchemasString(string schemas, JSchema rules)
        {
            return await Task.Run(() => Validate(new List<object> { schemas }, ForgeSchemaValidationRules));
        }

        public async Task<Tuple<bool, IList<string>>> ValidateSchemaInPath(string path, JSchema rules)
        {
            return await GetschemaFromPathAndValidate(path, rules);
        }

        public async Task<Tuple<bool, IList<string>>> ValidateMultipleSchemasInPath(string path, JSchema rules)
        {
            return await GetAllSchemasInDirectoryAndValidate(path, rules);
        }

        public async Task<Tuple<bool, IList<string>>> ValidateSchemaString(string schema, JSchema rules)
        {
            return await Task.Run(() => Validate(new List<object> { schema }, rules));
        }

        private static List<object> ConvertStringToSchemaList(string schemas, out bool success)
        {
            try
            {
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(schemas));
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<ForgeTree>));
                var schemaList = ser.ReadObject(ms) as List<object>;
                ms.Close();
                success = true;
                return schemaList;
            }
            catch
            {
                success = false;
                return new List<object>();
            }
        }
        private Task<Tuple<bool, IList<string>>> Validate(List<object> schemas, object rules)
        {
            var errorList = new List<string>();
            try
            {
                JSchema jRules;
                //input rules could be string, Jschema type. Null means using Default rules
                if (rules is null)
                    jRules = this.ForgeSchemaValidationRules;
                else if (rules is JSchema)
                    jRules = (JSchema)rules;
                else
                    jRules = JSchema.Parse((string)rules);

                var schemaList = new List<JObject>();
                foreach (var item in schemas)
                {
                    //schmeas type could be string or forgeTree
                    if (item is string)
                    {

                        Dictionary<string, ForgeTree> forgeTrees = JsonConvert.DeserializeObject<Dictionary<string, ForgeTree>>((string)item);
                        foreach (var kvp in forgeTrees)
                        {
                            ForgeTree forgeTree = kvp.Value;
                            if (forgeTree.Tree == null)
                            {
                                // Deserialize into Dictionary does not throw exception but will have null "Tree" property if schema is just a ForgeTree.
                                // try to deserialize string to forge tree directly
                                JsonConvert.DeserializeObject<ForgeTree>((string)item);
                                JObject schema = JObject.Parse((string)item);
                                schemaList.Add(schema);
                                break;
                            }
                            SerializeForgeTree(schemaList, forgeTree);
                        }

                    }
                    else
                    {
                        SerializeForgeTree(schemaList, (ForgeTree)item);
                    }
                }
                if (schemaList.Count == 0)
                {
                    errorList.Add("Can't find target schema to test or file type is not supported");
                    return Task.FromResult(new Tuple<bool, IList<string>>(false, errorList));
                }
                foreach (var item in schemaList)
                {
                    var validated = DoValidate(item, jRules, out IList<ValidationError> errors);
                    if (!validated)
                    {
                        foreach (var error in errors)
                        {
                            errorList.Add(error.Message + " line: " + error.LineNumber + " position: " + error.LinePosition);
                        }
                        return Task.FromResult(new Tuple<bool, IList<string>>(false, errorList));
                    }
                }
                return Task.FromResult(new Tuple<bool, IList<string>>(true, errorList));
            }
            catch (Exception e)
            {
                errorList.Add(e.Message);
                return Task.FromResult(new Tuple<bool, IList<string>>(false, errorList));
            }
        }

        private static void SerializeForgeTree(List<JObject> schemaList, ForgeTree forgeTree)
        {
            string stringSchema = JsonConvert.SerializeObject(
                forgeTree,
                new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore, // Prevent default values from getting added to serialized json schema.
                    Converters = new List<JsonConverter> { new Newtonsoft.Json.Converters.StringEnumConverter() } // Use string enum values instead of numerical.
                });
            schemaList.Add(JObject.Parse((string)stringSchema));
        }

        private async Task<Tuple<bool, IList<string>>> GetschemaFromPathAndValidate(string path, object rules)
        {
            try
            {
                var schema = File.ReadAllText(path);
                return await Task.Run(() => Validate(new List<object> { schema }, rules));
            }
            catch
            {
                return await Task.FromResult(new Tuple<bool, IList<string>>(false, new List<string> { "Can not get schema in string from given path" }));
            }
        }

        private async Task<Tuple<bool, IList<string>>> GetAllSchemasInDirectoryAndValidate(string path, object rules)
        {
            try
            {
                string[] Files = Directory.GetFiles(path);
                var schemalist = new List<object>();
                foreach (string file in Files)
                    schemalist.Add(File.ReadAllText(file));
                return await Task.Run(() => Validate(schemalist, rules));
            }
            catch
            {
                return await Task.FromResult(new Tuple<bool, IList<string>>(false, new List<string> { "Can not get schema in string from files in the given directory" }));
            }
        }

        private JSchema GetRulesFromPath(string path)
        {
            try
            {
                var jsonSchemaRules = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, path));
                return JSchema.Parse(jsonSchemaRules);
            }
            catch
            {
                return null;
            }
        }

        private bool DoValidate(JObject schema, JSchema rules, out IList<ValidationError> errorDetail)
        {
            bool isValid = schema.IsValid(rules, out IList<ValidationError> errors);
            errorDetail = errors;
            return isValid;
        }
        private static List<object> ConvertSchemasToSchemaList(IList<ForgeTree> schemas)
        {
            var schemaList = new List<object>();
            foreach (var schema in schemas)
            {
                schemaList.Add(schema);
            }
            return schemaList;
        }
    }
}
