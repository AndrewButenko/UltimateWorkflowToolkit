using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Security
{

    public class AddUserToTeam : CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("User")]
        [RequiredArgument]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> User { get; set; }

        [Input("Team")]
        [RequiredArgument]
        [ReferenceTarget("team")]
        public InArgument<EntityReference> Team { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context,
            IOrganizationService service, IOrganizationService sysService)
        {
            sysService.Execute(new AddMembersTeamRequest()
            {
                MemberIds = new[] { User.Get(executionContext).Id },
                TeamId = Team.Get(executionContext).Id
            });
        }
    }
}
