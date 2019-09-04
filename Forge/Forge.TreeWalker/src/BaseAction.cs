//-----------------------------------------------------------------------
// <copyright file="BaseAction.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The BaseAction abstract class.
// </summary>
//-----------------------------------------------------------------------

namespace Forge.TreeWalker
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The BaseAction abstract class must be inherited by all ForgeActions tagged with the ForgeActionAttribute.
    /// All ForgeActionAttribute tagged classes should reside in the Assembly that is passed to Forge TreeWalkerSession.
    /// When Forge encounters an ActionName while walking the tree, it will instantiate the ForgeAction type and then call RunAction.
    /// </summary>
    public abstract class BaseAction
    {
        /// <summary>
        /// The RunAction method is called when Forge encounters an ActionName while walking the tree.
        /// </summary>
        /// <param name="actionContext">The action context holding relevant information for this Action.</param>
        public abstract Task<ActionResponse> RunAction(ActionContext actionContext);
    }
}