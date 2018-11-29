using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using System.Collections.Generic;

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

        protected override void PerformOperation(List<Entity> childRecords, bool isContinueOnError)
        {
            foreach (var childRecord in childRecords)
            {
                try
                {
                    Context.UserService.Execute(new ExecuteWorkflowRequest()
                    {
                        EntityId = childRecord.Id,
                        WorkflowId = Workflow.Get(Context.ExecutionContext).Id
                    });
                }
                catch
                {
                    if (!isContinueOnError || Context.IsSyncMode)
                        throw;
                }
            }
        }
    }
}
