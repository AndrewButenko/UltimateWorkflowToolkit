using System.Activities;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

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

        public override void PerformRelationshipOperation(Entity childRecord)
        {
            Context.UserService.Execute(new ExecuteWorkflowRequest()
            {
                EntityId = childRecord.Id,
                WorkflowId = Workflow.Get(Context.ExecutionContext).Id
            });
        }
    }
}
