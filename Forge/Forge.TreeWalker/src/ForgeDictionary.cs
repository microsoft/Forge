//-----------------------------------------------------------------------
// <copyright file="ForgeDictionary.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The ForgeDictionary class.
// </summary>
//-----------------------------------------------------------------------

namespace Forge.TreeWalker
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The ForgeDictionary class defines the methods for accessing forge state.
    /// Note: The KeyPrefix should always precede the key when using the forgeStateTable to limit the scope to the current SessionId.
    /// </summary>
    public class ForgeDictionary : IForgeDictionary
    {
        /// <summary>
        /// The unique identifier for this session.
        /// </summary>
        public Guid SessionId { get; set; }

        /// <summary>
        /// The key prefix that should precede the keys when using the forgeStateTable.
        /// This ensures the scope of the table is limited to the current SessionId.
        /// </summary>
        private string keyPrefix;

        /// <summary>
        /// The dictionary holding the forge state.
        /// Maps the string key to object value.
        /// The key should always be preceded by the KeyPrefix. This ensures the scope of the table is limited to the current SessionId.
        /// </summary>
        private IDictionary<string, object> forgeStateTable;

        /// <summary>
        /// ForgeDictionary Constructor.
        /// </summary>
        /// <param name="forgeStateTable">The forge state table dictionary object.</param>
        /// <param name="sessionId">The unique identifier for this session.</param>
        public ForgeDictionary(IDictionary<string, object> forgeStateTable, Guid sessionId)
        {
            this.forgeStateTable = forgeStateTable;
            this.SessionId = sessionId;
            this.keyPrefix = this.SessionId + "_";
        }

        /// <summary>
        /// Sets an element with the provided key and value to the backing store.
        /// </summary>
        /// <param name="key">The key of the element to set.</param>
        /// <param name="value">The value of the element to be set.</param>
        public Task Set<T>(string key, T value)
        {
            this.forgeStateTable[this.keyPrefix + key] = (object)value;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Sets a list of key value pairs to the backing store.
        /// </summary>
        /// <param name="kvps">The list of key value pairs to set.</param>
        public Task SetRange<T>(List<KeyValuePair<string, T>> kvps)
        {
            foreach(KeyValuePair<string, T> kvp in kvps)
            {
                this.forgeStateTable[this.keyPrefix + kvp.Key] = (object)kvp.Value;
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets an element with the provided key from the backing store.
        /// </summary>
        /// <param name="key">The key of the element to get.</param>
        /// <returns>The value of the element to get.</returns>
        public Task<T> GetValue<T>(string key)
        {
            return Task.FromResult((T)this.forgeStateTable[this.keyPrefix + key]);
        }

        /// <summary>
        /// Removes an element with the provided key from the backing store.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>True of the element was removed, False otherwise.</returns>
        public Task<bool> RemoveKey(string key)
        {
            return Task.FromResult(this.forgeStateTable.Remove(this.keyPrefix + key));
        }

        /// <summary>
        /// Removes a list of elements with the provided keys from the backing store.
        /// </summary>
        /// <param name="keys">The list of keys to remove.</param>
        public Task RemoveKeys(List<string> keys)
        {
            foreach(string key in keys)
            {
                this.forgeStateTable.Remove(this.keyPrefix + key);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Sets an element with the provided key and value to the backing store.
        /// </summary>
        /// <param name="key">The key of the element to set.</param>
        /// <param name="value">The value of the element to be set.</param>
        public void SetSync<T>(string key, T value)
        {
            this.forgeStateTable[this.keyPrefix + key] = (object)value;
        }

        /// <summary>
        /// Sets a list of key value pairs to the backing store.
        /// </summary>
        /// <param name="kvps">The list of key value pairs to set.</param>
        public void SetRangeSync<T>(List<KeyValuePair<string, T>> kvps)
        {
            foreach(KeyValuePair<string, T> kvp in kvps)
            {
                this.forgeStateTable[this.keyPrefix + kvp.Key] = (object)kvp.Value;
            }
        }

        /// <summary>
        /// Gets an element with the provided key from the backing store.
        /// </summary>
        /// <param name="key">The key of the element to get.</param>
        /// <returns>The value of the element to get.</returns>
        public T GetValueSync<T>(string key)
        {
            return (T)this.forgeStateTable[this.keyPrefix + key];
        }

        /// <summary>
        /// Removes an element with the provided key from the backing store.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>True of the element was removed, False otherwise.</returns>
        public bool RemoveKeySync(string key)
        {
            return this.forgeStateTable.Remove(this.keyPrefix + key);
        }

        /// <summary>
        /// Removes a list of elements with the provided keys from the backing store.
        /// </summary>
        /// <param name="keys">The list of keys to remove.</param>
        public void RemoveKeysSync(List<string> keys)
        {
            foreach(string key in keys)
            {
                this.forgeStateTable.Remove(this.keyPrefix + key);
            }
        }
    }
}