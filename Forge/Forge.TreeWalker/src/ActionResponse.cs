//-----------------------------------------------------------------------
// <copyright file="ActionResponse.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The ActionResponse class.
// </summary>
//-----------------------------------------------------------------------

namespace Forge.TreeWalker
{
    /// <summary>
    /// The ActionResponse class holds the response information from actions.
    /// </summary>
    public class ActionResponse
    {
        /// <summary>
        /// The status code of this action response.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// The status of this action response.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The dynamic output of this action response.
        /// </summary>
        public object Output { get; set; }
    }
}