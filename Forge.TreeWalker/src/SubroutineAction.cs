//-----------------------------------------------------------------------
// <copyright file="SubroutineAction.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The SubroutineAction class.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.TreeWalker
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Forge.Attributes;

    /// <summary>
    /// The SubroutineAction is a native Forge action that walks a TreeWalkerSession.
    /// This action performs the following steps:
    ///   1. Rehydrates the Subroutine's SessionId if previously persisted.
    ///   2. Gets an initialized TreeWalkerSession for this Subroutine from the App, passing in the SubroutineInput from the schema and Subroutine SessionId.
    ///   3. Explicitly calls UpdateKeyPrefix on the ForgeState with the updated Subroutine SessionId.
    ///   4. Walks the tree starting at the CurrentTreeNode if persisted, otherwise RootTreeNodeKey if given, otherwise "Root" as default.
    ///   5. Returns the Subroutine's last ActionResponse if available, otherwise returns the tree walker session Status.
    /// </summary>
    [ForgeAction(InputType: typeof(SubroutineInput))]
    public class SubroutineAction : BaseAction
    {
        /// <summary>
        /// The tree walker parameters of the parent tree walker session.
        /// Used to call InitializeSubroutineTree to get the initialized TreeWalkerSession for this Subroutine.
        /// </summary>
        private readonly TreeWalkerParameters parameters;

        /// <summary>
        /// Instantiates a SubroutineAction with the required parameters.
        /// </summary>
        /// <param name="parameters">The tree walker parameters of the parent tree walker session.</param>
        public SubroutineAction(TreeWalkerParameters parameters)
        {
            this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        /// <summary>
        /// The RunAction method is called when Forge encounters an ActionName while walking the tree.
        /// </summary>
        /// <param name="actionContext">The action context holding relevant information for this Action.</param>
        public override async Task<ActionResponse> RunAction(ActionContext actionContext)
        {
            SubroutineInput input = (SubroutineInput)actionContext.ActionInput;

            // Rehydrate the subroutine's SessionId if previously persisted.
            SubroutineIntermediates intermediates = await actionContext.GetIntermediates<SubroutineIntermediates>();
            if (intermediates == null)
            {
                intermediates = new SubroutineIntermediates()
                {
                    SessionId = Guid.NewGuid()
                };

                await actionContext.CommitIntermediates<SubroutineIntermediates>(intermediates);
            }

            // Initialize TreeWalkerSession for this subroutine.
            TreeWalkerSession subroutineSession = this.parameters.InitializeSubroutineTree(input, intermediates.SessionId, this.parameters);

            // Update KeyPrefix of ForgeState for state persistence separation.
            subroutineSession.Parameters.ForgeState.UpdateKeyPrefix(subroutineSession.Parameters.RootSessionId, subroutineSession.Parameters.SessionId);

            // Walk tree starting at CurrentTreeNode if persisted, otherwise RootTreeNodeKey if given, otherwise "Root" as default.
            string currentTreeNode = await subroutineSession.GetCurrentTreeNode() ?? subroutineSession.Schema.RootTreeNodeKey;

            // WalkTree may throw exceptions. We let this happen to allow for possible retry handling.
            await subroutineSession.WalkTree(currentTreeNode);

            // Return the subroutine's last action response if available, otherwise return tree walker Status.
            return await subroutineSession.GetLastActionResponseAsync() ?? new ActionResponse() { Status = subroutineSession.Status };
        }
    }

    /// <summary>
    /// The Subroutine intermediates object that gets persisted with the Subroutine's SessionId.
    /// </summary>
    public class SubroutineIntermediates
    {
        /// <summary>
        /// The SessionId of this Subroutine tree walker session.
        /// </summary>
        public Guid SessionId { get; set; }
    }

    /// <summary>
    /// The Subroutine input object that gets instantiated from the schema.
    /// </summary>
    public class SubroutineInput
    {
        /// <summary>
        /// The TreeName mapping to a known ForgeTree in the App.
        /// </summary>
        public string TreeName { get; set; }

        /// <summary>
        /// (Optional) The dynamic TreeInput object for this tree walking session.
        /// TreeInput is able to be referenced when evaluating schema expressions.
        /// </summary>
        public object TreeInput { get; set; }

        /// <summary>
        /// (Optional) The path where the schema file resides.
        /// </summary>
        public string TreeFilePath { get; set; }
    }
}
