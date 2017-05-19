using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Security
{
    public class AddUserToRecordTeam : CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Record Reference")]
        [RequiredArgument]
        public InArgument<string> Record { get; set; }

        [Input("User")]
        [RequiredArgument]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> User { get; set; }

        [Input("Team Template")]
        [RequiredArgument]
        [ReferenceTarget("teamtemplate")]
        public InArgument<EntityReference> TeamTemplate { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context,
            IOrganizationService service, IOrganizationService sysService)
        {
            var target = ConvertToEntityReference(Record.Get(executionContext), service);

            sysService.Execute(new AddUserToRecordTeamRequest()
            {
                Record = target,
                SystemUserId = User.Get(executionContext).Id,
                TeamTemplateId = TeamTemplate.Get(executionContext).Id
            });
        }
    }
}
