using System.Activities;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.CoreOperations.Base;
using Microsoft.Crm.Sdk.Messages;

namespace UltimateWorkflowToolkit.CoreOperations.Views
{
    public class ViewDistributeWorkflow : ViewOperationWorkflowBase
    {

        #region Inputs/Outputs

        [Input("Distributed Workflow")]
        [ReferenceTarget("workflow")]
        [RequiredArgument]
        public InArgument<EntityReference> Workflow { get; set; }

        #endregion Inputs/Outputs

        #region Overriddes

        protected override void ProcessRecords(List<Entity> records)
        {
            var workflow = Workflow.Get(Context.ExecutionContext).Id;

            records.ForEach(t => Context.UserService.Execute(new ExecuteWorkflowRequest()
            {
                EntityId = t.Id,
                WorkflowId = workflow
            }));
        }

        #endregion Overriddes
    }
}
