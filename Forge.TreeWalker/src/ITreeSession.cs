//-----------------------------------------------------------------------
// <copyright file="ITreeSession.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The ITreeSession interface.
// </summary>
//-----------------------------------------------------------------------

namespace Forge.TreeWalker
{
    using System.Threading.Tasks;

    /// <summary>
    /// The ITreeSession interface holds accessor methods into the forgeState dictionary.
    /// </summary>
    public interface ITreeSession
    {
        /// <summary>
        /// Gets the ActionResponse data from the forgeState for the given tree action key.
        /// </summary>
        /// <param name="treeActionKey">The TreeAction's key of the action that was executed.</param>
        /// <returns>The ActionResponse data for the given tree action key if it exists, otherwise null.</returns>
        ActionResponse GetOutput(string treeActionKey);

        /// <summary>
        /// Asynchronously gets the ActionResponse data from the forgeState for the given tree action key.
        /// </summary>
        /// <param name="treeActionKey">The TreeAction's key of the action that was executed.</param>
        /// <returns>The ActionResponse data for the given tree action key if it exists, otherwise null.</returns>
        Task<ActionResponse> GetOutputAsync(string treeActionKey);

        /// <summary>
        /// Gets the last executed TreeAction's ActionResponse data from the forgeState.
        /// </summary>
        /// <returns>The ActionResponse data for the last executed tree action key if it exists, otherwise null.</returns>
        ActionResponse GetLastActionResponse();

        /// <summary>
        /// Asynchronously gets the last executed TreeAction's ActionResponse data from the forgeState.
        /// </summary>
        /// <returns>The ActionResponse data for the last executed tree action key if it exists, otherwise null.</returns>
        Task<ActionResponse> GetLastActionResponseAsync();
    }
}