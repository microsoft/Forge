//-----------------------------------------------------------------------
// <copyright file="ForgeActionAttribute.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The ForgeActionAttribute class.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.Attributes
{
    using System;

    /// <summary>
    /// The ForgeActionAttribute class defines the ForgeAction attribute.
    ///
    /// This attribute should be added to all Forge Action classes that wish to be called by Forge while walking the tree.
    /// The Name of the Action class will be used in the ForgeTree schema file to map to the Action with this ForgeAction attribute.
    /// Action classes must inherit from Forge's BaseAction class, as Forge will call the Setup and RunAction methods on these classes.
    ///
    /// InputType should be included if the Action wishes for Forge to instantiate that object from the ForgeSchema and pass it to the Action.
    /// Otherwise, Forge will create a dynamic object from the Input in ForgeSchema if it exists, and pass it to the Action.
    ///
    /// Ex) [ForgeAction(InputType: typeof(FooInput))]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ForgeActionAttribute : Attribute
    {
        /// <summary>
        /// InputType should be included if the Action wishes for Forge to instantiate that object from the ForgeSchema and pass it to the Action.
        /// If null, Forge will create a dynamic object from the Input in ForgeSchema if it exists and pass it to the Action.
        /// </summary>
        public Type InputType { get; private set; }

        /// <summary>
        /// Instantiates a ForgeActionAttribute.
        /// </summary>
        /// <param name="InputType">The input Type for this Action.</param>
        public ForgeActionAttribute(Type InputType)
        {
            this.InputType = InputType;
        }

        /// <summary>
        /// Instantiates a ForgeActionAttribute.
        /// </summary>
        public ForgeActionAttribute()
        : this(null)
        {
        }
    }
}