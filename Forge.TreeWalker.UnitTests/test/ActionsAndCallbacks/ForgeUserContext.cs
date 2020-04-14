//-----------------------------------------------------------------------
// <copyright file="ForgeUserContext.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The ForgeUserContext class is passed in to TreeWalkerParameters to be used in the schema and ForgeActions as the UserContext object.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.TreeWalker.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ForgeUserContext
    {
        public string Name { get; set; } = "MyName";
        public string ResourceType { get; set; } = "Container";
        public FooActionInput CustomObject { get; set; } = new FooActionInput()
            {
                Command = "TheCommand",
                NestedObject = new FooActionObject()
                {
                    IntPropertyInObject = 10
                },
                ObjectArray = new FooActionObject[]
                {
                    new FooActionObject()
                    {
                        Name = "MyName"
                    }
                }
            };

        public int GetCount()
        {
            return 1;
        }

        public Task<int> GetCountAsync()
        {
            return Task.FromResult(2);
        }

        public IDictionary<string, string> GetDictionary()
        {
            return new Dictionary<string, string>()
            {
                {
                    "Key1", "Value1"
                },
                {
                    "Key2", "Value2"
                }
            };
        }

        public IDictionary<string, FooActionInput> GetCustomObjectDictionary()
        {
            return new Dictionary<string, FooActionInput>()
            {
                {
                    "Key1", this.CustomObject
                }
            };
        }

        public FooActionInput[] GetCustomObjectArray()
        {
            return new FooActionInput[]
            {
                this.CustomObject,
                this.CustomObject
            };
        }
    }
}
