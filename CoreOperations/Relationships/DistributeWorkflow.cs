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

        public override void PerformRelationshipOperation(CodeActivityContext executionContext, IOrganizationService service, Entity childRecord)
        {
            service.Execute(new ExecuteWorkflowRequest()
            {
                EntityId = childRecord.Id,
                WorkflowId = Workflow.Get(executionContext).Id
            });
        }
    }
}
