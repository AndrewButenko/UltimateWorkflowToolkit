using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class Delete: CrmWorkflowBase
    {
        #region Input/Output Arguments

        [Input("Record Url")]
        [RequiredArgument]
        public InArgument<string> RecordUrl { get; set; }

        #endregion Input/Output Arguments

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service)
        {
            var target = ParseUrlToEntityReference(RecordUrl.Get(executionContext), service);

            service.Delete(target.LogicalName, target.Id);
        }
    }
}
