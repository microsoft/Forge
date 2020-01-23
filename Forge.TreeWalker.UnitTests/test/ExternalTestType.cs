//-----------------------------------------------------------------------
// <copyright file="ExternalTestType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Test type used to test external Forge dependency injection.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.ExternalTypes
{

    public enum ExternalTestType
    {
        ExampleEnum = 0,

        TestEnum = 1,
    }

    public enum TestType
    {
        Example = 0,

        Test = 1
    }
}

namespace Microsoft.Forge.TreeWalker.UnitTests
{
    public enum DiffNamespaceType
    {
        TestOne,

        TestTwo
    }
}