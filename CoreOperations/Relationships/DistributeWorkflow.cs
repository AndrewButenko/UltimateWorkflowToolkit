using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;

namespace UltimateWorkflowToolkit.CoreOperations.Relationships
{
    public class DistributeWorkflow : RelationshipOperationBase
    {
        #region Input Parameters

        [Input("Distributed Workflow")]
        [ReferenceTarget("workflow")]
        [RequiredArgument]
        public InArgument<EntityReference> Workflow { get; set; }

        #endregion Input Parameters

        protected override void PerformRelationshipOperation(Entity childRecord)
        {
            Context.UserService.Execute(new ExecuteWorkflowRequest()
            {
                EntityId = childRecord.Id,
                WorkflowId = Workflow.Get(Context.ExecutionContext).Id
            });
        }
    }
}
