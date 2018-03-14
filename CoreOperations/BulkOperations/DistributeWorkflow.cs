using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;

namespace UltimateWorkflowToolkit.CoreOperations.BulkOperations
{
    public class DistributeWorkflow : BulkOperationBase
    {
        #region Input Parameters

        [Input("Distributed Workflow")]
        [ReferenceTarget("workflow")]
        [RequiredArgument]
        public InArgument<EntityReference> Workflow { get; set; }

        #endregion Input Parameters

        protected override void PerformOperation(Entity childRecord)
        {
            Context.UserService.Execute(new ExecuteWorkflowRequest()
            {
                EntityId = childRecord.Id,
                WorkflowId = Workflow.Get(Context.ExecutionContext).Id
            });
        }
    }
}
