using System.Activities;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Security
{
    public class UnshareRecordWithUser : CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Record Reference")]
        [RequiredArgument]
        public InArgument<string> Record { get; set; }

        [Input("User")]
        [RequiredArgument]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> User { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context,
            IOrganizationService service, IOrganizationService sysService)
        {
            var target = ConvertToEntityReference(Record.Get(executionContext), service);

            sysService.Execute(new RevokeAccessRequest()
            {
                Revokee = User.Get(executionContext),
                Target = target
            });
        }
    }
}
