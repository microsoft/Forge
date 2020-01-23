//-----------------------------------------------------------------------
// <copyright file="ForgeTree.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The Forge schema data contracts.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The Forge tree.
    /// This outermost data structure holds the Forge schema.
    /// </summary>
    [DataContract]
    public class ForgeTree
    {
        /// <summary>
        /// Dictionary mapping unique TreeNodeKeys to TreeNodes.
        /// </summary>
        [DataMember]
        public Dictionary<string, TreeNode> Tree { get; set; }

        /// <summary>
        /// The root TreeNodeKey that should be visited first when walking the tree.
        /// </summary>
        [DataMember]
        public string RootTreeNodeKey { get; set; } = "Root";
    }

    /// <summary>
    /// The tree node.
    /// Holds information to navigate the tree and perform actions.
    /// </summary>
    [DataContract]
    public class TreeNode
    {
        /// <summary>
        /// The tree node type.
        /// </summary>
        [DataMember(IsRequired = true)]
        public TreeNodeType Type { get; private set; }

        /// <summary>
        /// Additional properties passed to wrapper class.
        /// String properties starting with <see cref="TreeWalkerSession.RoslynLeadingText"/> represent a code-snippet that will be evaluated.
        /// </summary>
        [DataMember]
        public dynamic Properties { get; set; }

        /// <summary>
        /// The child selectors.
        /// </summary>
        [DataMember]
        public ChildSelector[] ChildSelector { get; private set; }

        #region Properties used only by TreeNodeType.Action nodes

        /// <summary>
        /// The actions to execute when the TreeNodeType is Action.
        /// Dictionary mapping unique TreeActionKeys to TreeActions.
        /// </summary>
        [DataMember]
        public Dictionary<string, TreeAction> Actions { get; set; }

        /// <summary>
        /// Timeout in milliseconds for executing the TreeActions. Default to -1 (infinite) if not specified.
        /// String properties starting with <see cref="TreeWalkerSession.RoslynLeadingText"/> represent a code-snippet that will be evaluated.
        /// </summary>
        [DataMember]
        public dynamic Timeout { get; set; }

        #endregion
    }

    /// <summary>
    /// The child selector for the TreeNode.
    /// Used to navigate the tree by referencing child TreeNodes.
    /// </summary>
    [DataContract]
    public class ChildSelector
    {
        /// <summary>
        /// String code-snippet that can be parsed and evaluated to a boolean value.
        /// If the expression is true, visit the attached child TreeNode.
        /// If the expression is empty, evaluate to true by default.
        /// </summary>
        [DataMember]
        public string ShouldSelect { get; set; }

        /// <summary>
        /// Reader-friendly label that describes the intention of the ShouldSelect expression.
        /// Used in ForgeEditor for display purposes.
        /// </summary>
        [DataMember]
        public string Label { get; set; }

        /// <summary>
        /// String key pointer to a child TreeNode.
        /// Visit this child if the attached ShouldSelect expression evaluates to true.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Child { get; private set; }
    }

    /// <summary>
    /// The tree action for the TreeNode.
    /// Holds instructions and policies for executing an action.
    /// </summary>
    [DataContract]
    public class TreeAction
    {
        /// <summary>
        /// String name of the action that maps to an action-task.
        /// These actions may be predefined Forge actions or action-tasks passed by a Wrapper class.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Action { get; set; }

        /// <summary>
        /// Dynamic input parameters passed to the action-task.
        /// Wrapper class is responsible for making sure the action-task input matches the input defined in the schema.
        /// String properties starting with <see cref="TreeWalkerSession.RoslynLeadingText"/> represent a code-snippet that will be evaluated.
        /// </summary>
        [DataMember]
        public dynamic Input { get; private set; }

        /// <summary>
        /// Additional properties passed to wrapper class.
        /// String properties starting with <see cref="TreeWalkerSession.RoslynLeadingText"/> represent a code-snippet that will be evaluated.
        /// </summary>
        [DataMember]
        public dynamic Properties { get; set; }

        /// <summary>
        /// Timeout in milliseconds for executing the action. Default to -1 (infinite) if not specified.
        /// String properties starting with <see cref="TreeWalkerSession.RoslynLeadingText"/> represent a code-snippet that will be evaluated.
        /// </summary>
        [DataMember]
        public dynamic Timeout { get; set; }

        /// <summary>
        /// A flag that represents how to handle the exit of the action due to timeout. If false (default), then the session will end on the
        /// timeout. If true and a timeout is hit, the action will continue on as if it were successful after committing a "TimeoutOnAction" response.
        /// </summary>
        [DataMember]
        public bool ContinuationOnTimeout { get; set; }

        /// <summary>
        /// Retry policy of the action.
        /// </summary>
        [DataMember]
        public RetryPolicy RetryPolicy { get; private set; }

        /// <summary>
        /// A flag that represents how to handle the exit of the action due to retry exhaustion. If false (default), then the session will end once
        /// retries are exhausted or no retries are specified. If true and retries are exhausted, the action will continue on as if it were successful 
        /// after committing a "RetryExhaustedOnAction" response.
        /// </summary>
        [DataMember]
        public bool ContinuationOnRetryExhaustion { get; set; }
    }

    /// <summary>
    /// The retry policy for the TreeAction.
    /// </summary>
    [DataContract]
    public class RetryPolicy
    {
        /// <summary>
        /// The retry policy type.
        /// </summary>
        [DataMember(IsRequired = true)]
        public RetryPolicyType Type { get; private set; }

        /// <summary>
        /// Minimum backoff time in milliseconds.
        /// When retrying an action, wait at least this long before your next attempt.
        /// This is useful to ensure actions are not retried too quickly.
        /// </summary>
        [DataMember]
        public long MinBackoffMs { get; private set; }

        /// <summary>
        /// Maximum backoff time in milliseconds.
        /// When retrying an action, wait at most this long before your next attempt.
        /// This is useful to ensure exponential backoff doesn't wait too long.
        /// </summary>
        [DataMember]
        public long MaxBackoffMs { get; private set; }
    }

    /// <summary>
    /// The retry policy types.
    /// </summary>
    [DataContract]
    public enum RetryPolicyType
    {
        /// <summary>
        /// Do not retry.
        /// </summary>
        [EnumMember]
        None = 0,

        /// <summary>
        /// Retry at a fixed interval every MinBackoffMs.
        /// </summary>
        [EnumMember]
        FixedInterval = 1,

        /// <summary>
        /// Retry with an exponential backoff.
        /// Start with MinBackoffMs, then wait Math.Min(MinBackoffMs * 2^(retryCount), MaxBackoffMs).
        /// </summary>
        [EnumMember]
        ExponentialBackoff = 2

        // TODO: Add a FixedCount type that will give the full timeout duration for the set number of retries.
    }

    /// <summary>
    /// The tree node types.
    /// </summary>
    [DataContract]
    public enum TreeNodeType
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Selection type node.
        /// </summary>
        [EnumMember]
        Selection = 1,

        /// <summary>
        /// Action type node.
        /// This node includes TreeAction(s).
        /// </summary>
        [EnumMember]
        Action = 2,

        /// <summary>
        /// Leaf type node.
        /// This represents an end state in tree.
        /// </summary>
        [EnumMember]
        Leaf = 3,

        /// <summary>
        /// Subroutine type node.
        /// This node contains at least one SubroutineAction.
        /// </summary>
        [EnumMember]
        Subroutine = 4
    }
}