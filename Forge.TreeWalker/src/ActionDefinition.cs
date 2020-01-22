//-----------------------------------------------------------------------
// <copyright file="ActionDefinition.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The ActionDefinition class.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.TreeWalker
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The ActionDefinition class holds definitions for the action.
    /// </summary>
    public class ActionDefinition
    {
        /// <summary>
        /// The Type of the ForgeAction class.
        /// </summary>
        public Type ActionType { get; set; }

        /// <summary>
        /// The InputType for this Action that will be passed to the Action by Forge when executing the Action.
        /// When given, Forge will instantiate this type when evaluating the ActionInput from the schema.
        /// When null, Forge will instead create a dynamic object.
        /// Restrictions: Only the public Properties of the InputType will be instantiated.
        ///               Objects lacking a parameterless constructor are not supported.
        ///               Objects with public fields are not supported.
        /// </summary>
        public Type InputType { get; set; }
    }
}