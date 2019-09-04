//-----------------------------------------------------------------------
// <copyright file="IForgeDictionary.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The IForgeDictionary interface.
// </summary>
//-----------------------------------------------------------------------

namespace Forge.TreeWalker
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The IForgeDictionary interface defines the methods for accessing forge state.
    /// Note: The KeyPrefix should always precede the key when using the forgeStateTable to limit the scope to the current SessionId.
    /// </summary>
    public interface IForgeDictionary
    {
        /// <summary>
        /// The unique identifier for this session.
        /// </summary>
        Guid SessionId { get; set; }

        /// <summary>
        /// Sets an element with the provided key and value to the backing store.
        /// </summary>
        /// <param name="key">The key of the element to set.</param>
        /// <param name="value">The value of the element to be set.</param>
        Task Set<T>(string key, T value);

        /// <summary>
        /// Sets a list of key value pairs to the backing store.
        /// </summary>
        /// <param name="kvps">The list of key value pairs to set.</param>
        Task SetRange<T>(List<KeyValuePair<string, T>> kvps);

        /// <summary>
        /// Gets an element with the provided key from the backing store.
        /// </summary>
        /// <param name="key">The key of the element to get.</param>
        /// <returns>The value of the element to get.</returns>
        Task<T> GetValue<T>(string key);

        /// <summary>
        /// Removes an element with the provided key from the backing store.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>True of the element was removed, False otherwise.</returns>
        Task<bool> RemoveKey(string key);

        /// <summary>
        /// Removes a list of elements with the provided keys from the backing store.
        /// </summary>
        /// <param name="keys">The list of keys to remove.</param>
        Task RemoveKeys(List<string> keys);
    }
}