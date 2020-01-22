//-----------------------------------------------------------------------
// <copyright file="BaseCommonAction.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     The BaseCommonAction abstract class implements the BaseAction abstract class.
// </summary>
//-----------------------------------------------------------------------

namespace Forge.TreeWalker.UnitTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Forge.TreeWalker;

    public abstract class BaseCommonAction : BaseAction
    {
        public object Input { get; private set; }

        public CancellationToken Token { get; private set; }

        public Guid SessionId { get; private set; }

        public string TreeNodeKey { get; private set; }

        private ActionContext actionContext;

        public override Task<ActionResponse> RunAction(ActionContext actionContext)
        {
            this.Input = actionContext.ActionInput;
            this.Token = actionContext.Token;
            this.SessionId = actionContext.SessionId;
            this.TreeNodeKey = actionContext.TreeNodeKey;
            this.actionContext = actionContext;

            return this.RunAction();
        }

        public abstract Task<ActionResponse> RunAction();

        public Task CommitIntermediates<T>(T intermediates)
        {
            return this.actionContext.CommitIntermediates<T>(intermediates);
        }

        public Task<T> GetIntermediates<T>()
        {
            return this.actionContext.GetIntermediates<T>();
        }

        public Task<ActionResponse> GetPreviousActionResponse()
        {
            return this.actionContext.GetPreviousActionResponse();
        }
    }
}
