//-----------------------------------------------------------------------
// <copyright file="ITreeSchemaValidator.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The ITreeSchemaValidator interface.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.TreeWalker
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Forge.DataContracts;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Schema;

    /// <summary>
    /// [DEPRECATED]
    /// The ITreeSchemaValidator interface defines the validation method that tests input schemas with the existing ForgeSchemaValidationRules or a customized rule from input.
    /// </summary>
    public interface ITreeSchemaValidator
    {
        /// <summary>
        /// The validate task that validate the input schema with ForgeSchemaValidationRules.
        /// </summary>
        /// <param name="schema">The schema to be validated</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        Task<Tuple<bool, IList<string>>> ValidateSchema(ForgeTree schema);

        /// <summary>
        /// The validate task that validate multiple input schemas with ForgeSchemaValidationRules.
        /// </summary>
        /// <param name="schemas">The schemas to be validated</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        Task<Tuple<bool, IList<string>>> ValidateSchemas(IList<ForgeTree> schemas);

        /// <summary>
        /// The validate task that check the input schema in string with ForgeSchemaValidationRules.
        /// </summary>
        /// <param name="schema">The schema to be validated</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        Task<Tuple<bool, IList<string>>> ValidateSchemaString(string schema);

        /// <summary>
        /// The validate task that validate multiple input schema in string with ForgeSchemaValidationRules.
        /// </summary>
        /// <param name="schemas">The schemas to be validated</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        Task<Tuple<bool, IList<string>>> ValidateSchemasString(string schemas);

        /// <summary>
        /// The validate task that validate the schema in the input file path with ForgeSchemaValidationRules.
        /// </summary>
        /// <param name="path">The path that contains a schema file</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        Task<Tuple<bool, IList<string>>> ValidateSchemaInPath(string path);

        /// <summary>
        /// The validate task that validate all schemas in a directory with ForgeSchemaValidationRules.
        /// </summary>
        /// <param name="path">The path that contains a schemas directory</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        Task<Tuple<bool, IList<string>>> ValidateMultipleSchemasInPath(string path);

        /// <summary>
        /// The validate task that validate the input schema with custom rules in string.
        /// </summary>
        /// <param name="schema">The schema to be validated</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        Task<Tuple<bool, IList<string>>> ValidateSchema(ForgeTree schema, string rules);

        /// <summary>
        /// The validate task that validate multiple input schemas with custom rules in string.
        /// </summary>
        /// <param name="schemas">The schemas to be validated</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        Task<Tuple<bool, IList<string>>> ValidateSchemas(IList<ForgeTree> schemas, string rules);

        /// <summary>
        /// The validate task that check the input schema in string with custom rules in string.
        /// </summary>
        /// <param name="schema">The schema to be validated</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        Task<Tuple<bool, IList<string>>> ValidateSchemaString(string schema, string rules);

        /// <summary>
        /// The validate task that validate multiple input schema in string with custom rules in string.
        /// </summary>
        /// <param name="schemas">The schemas to be validated</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        Task<Tuple<bool, IList<string>>> ValidateSchemasString(string schemas, string rules);

        /// <summary>
        /// The validate task that validate the schema in the input file path with custom rules in string.
        /// </summary>
        /// <param name="path">The path that contains a schema file</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        Task<Tuple<bool, IList<string>>> ValidateSchemaInPath(string path, string rules);

        /// <summary>
        /// The validate task that validate all schemas in a directory with custom rules in string.
        /// </summary>
        /// <param name="path">The path that contains a schemas directory</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        Task<Tuple<bool, IList<string>>> ValidateMultipleSchemasInPath(string path, string rules);

        /// <summary>
        /// The validate task that validate the input schema with custom rules in JSchema.
        /// </summary>
        /// <param name="schema">The schema to be validated</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        Task<Tuple<bool, IList<string>>> ValidateSchema(ForgeTree schema, JSchema rules);

        /// <summary>
        /// The validate task that validate multiple input schemas with custom rules in JSchema.
        /// </summary>
        /// <param name="schemas">The schemas to be validated</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        Task<Tuple<bool, IList<string>>> ValidateSchemas(IList<ForgeTree> schemas, JSchema rules);

        /// <summary>
        /// The validate task that check the input schema in string with custom rules in JSchema.
        /// </summary>
        /// <param name="schema">The schema to be validated</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        Task<Tuple<bool, IList<string>>> ValidateSchemaString(string schema, JSchema rules);

        /// <summary>
        /// The validate task that validate multiple input schema in string with custom rules in JSchema.
        /// </summary>
        /// <param name="schemas">The schemas to be validated</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        Task<Tuple<bool, IList<string>>> ValidateSchemasString(string schemas, JSchema rules);

        /// <summary>
        /// The validate task that validate the schema in the input file path with custom rules in JSchema.
        /// </summary>
        /// <param name="path">The path that contains a schema file</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        Task<Tuple<bool, IList<string>>> ValidateSchemaInPath(string path, JSchema rules);

        /// <summary>
        /// The validate task that validate all schemas in a directory with custom rules in JSchema.
        /// </summary>
        /// <param name="path">The path that contains a schemas directory</param>
        /// <param name="rules">The rules used to validate input schemas</param>
        /// <returns>The result of schema validation. The errorList would contain error message if validation fails</returns>
        Task<Tuple<bool, IList<string>>> ValidateMultipleSchemasInPath(string path, JSchema rules);
    }
}