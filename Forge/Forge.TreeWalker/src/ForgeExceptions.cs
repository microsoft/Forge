//-----------------------------------------------------------------------
// <copyright file="ForgeExceptions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The ForgeExceptions.
// </summary>
//-----------------------------------------------------------------------

namespace Forge.TreeWalker.ForgeExceptions
{
    using System;

    /// <summary>
    /// Exception thrown on action timeout.
    /// </summary>
    public class ActionTimeoutException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionTimeoutException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ActionTimeoutException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionTimeoutException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public ActionTimeoutException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    /// <summary>
    /// Exception thrown when ChildSelector fails to select any child.
    /// </summary>
    public class NoChildMatchedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoChildMatchedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public NoChildMatchedException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Exception thrown when EvaluateDynamicProperty fails.
    /// </summary>
    public class EvaluateDynamicPropertyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluateDynamicPropertyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public EvaluateDynamicPropertyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluateDynamicPropertyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public EvaluateDynamicPropertyException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
