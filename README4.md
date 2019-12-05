## What is Forge?
Forge is a config-driven decision tree, designed to execute your logical workflows in a dynamic way.

* The config-driven approach allows for high versatility and velocity as compared to code deployments.
* The decision tree concept allows for better clarity and control of the workflows.
* The dynamic capabilities allows for high extensibility on top of the base features.

## Is Forge right for my application?
Forge is a perfect library for managing workflows that 1) make decisions and 2) update frequently.
Example scenarios: repair workflow driving, policy matching

## Use case: Azure-Compute Fault Handling
Microsoft Azure-Compute's fault handling service is backed by Forge. Fault information is passed to the TreeWalker and a ForgeTree schema is walked. Depending on the type of fault and a handful of other checks, TreeWalker may attempt in-place recovery actions. If these actions were unsuccessful in mitigating the issue, diagnostics are collected and Tardigrade may be performed. You can read more about Azure’s recovery workflow in this recent blogpost: https://azure.microsoft.com/en-us/blog/improving-azure-virtual-machines-resiliency-with-project-tardigrade/

## Roslyn - dynamically compiled expressions
Forge will dynamically compile and run C# code snippets from the JSON schema file using Roslyn. The Roslyn code has access to the Forge Session which holds all the persisted state output from Actions, and Forge UserContext, the dynamic user-defined object that can have direct access to your application. This effectively allows you to write C# code in a JSON schema file that controls your workflows. Powerful stuff!

## Forge components at a high-level
Forge has 3 major components: ForgeTree, TreeWalker, and ForgeEditor.
* ForgeTree is the JSON schema data contract that defines the tree structure. It contains normal tree-concept objects such as TreeNodes and ChildSelectors, as well as TreeActions and RetryPolicies.

* TreeWalker takes in the ForgeTree and other parameters, and walks the tree to completion. It calls user-defined callbacks and actions, passing in properties from the ForgeTree and the dynamic UserContext object. Instead of hardcoded statements in the code, the TreeWalker makes decisions at run-time about the path it walks by utilizing Roslyn to evaluate C# code-snippets from the ForgeTree.

* ForgeEditor is coming to GitHub by end of 2019 (currently only available internally to Microsoft). ForgeEditor is an Electron application that allows you to visualize and edit the ForgeTree in a clean UI experience. It contains features such as: tree visualization, buttons to create/delete TreeNodes, auto-complete when editing JSON file, text highlighting when hovering over TreeNode, evaluates ForgeSchemaValidationRules while editing, Diagnose mode, etc..

## Further Reading
Check out the [Wiki page](https://github.com/microsoft/Forge/wiki) for a deeper dive into Forge, as well as How To guides!

## Contributing
Interested in contributing to Forge? Check out the [Contributing](CONTRIBUTING.md) page for details.

## License
Copyright (c) Microsoft Corporation. All rights reserved.

Licensed under the [MIT](LICENSE.txt) license.
