//-----------------------------------------------------------------------
// <copyright file="TreeSchemaValidator.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The TreeSchemaValidator class implements the ITreeSchemaValidator interface.
// </summary>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Forge.DataContracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;


namespace Microsoft.Forge.TreeWalker.UnitTests.test
{
    class TreeSchemaValidator : ITreeSchemaValidator
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
            return await ValidateMultipleSchemasInString(schemas, ForgeSchemaValidationRules);
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
            return await ValidateMultipleSchemasInString(schemas, rules);
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
            return await ValidateMultipleSchemasInString(schemas, rules);
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
            return await Task.Run(() => Validate(new List<object>{schema}, rules));
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
                if (rules is null)
                    jRules = this.ForgeSchemaValidationRules;
                else if (rules is JSchema)
                    jRules = (JSchema)rules;
                else
                    jRules = JSchema.Parse((string)rules);

                foreach (var item in schemas)
                {
                    JObject schema;
                    if (item is string)
                        schema = JObject.Parse((string)item);
                    else
                        schema = (JObject)JToken.FromObject(item);
                    var validated = DoValidate(schema, jRules, out IList<ValidationError> errors);
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
                errorList.Add(e.InnerException.Message);
                return Task.FromResult(new Tuple<bool, IList<string>>(false, errorList));
            }
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
        private async Task<Tuple<bool, IList<string>>> ValidateMultipleSchemasInString(string schemas, object rules)
        {
            List<object> schemaList = ConvertStringToSchemaList(schemas, out bool success);
            if (success)
                return await Task.Run(() => Validate(schemaList, rules));
            else
                return await Task.FromResult(new Tuple<bool, IList<string>>(false, new List<string> { "Can't not convert string to ForgeTree object" }));
        }
    }
}
