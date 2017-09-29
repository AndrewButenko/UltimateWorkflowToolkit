using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Security
{
    public class RemoveUserFromRecordTeam : CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Record")]
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

        protected override void ExecuteWorkflowLogic()
        {
            var target = ConvertToEntityReference(Record.Get(Context.ExecutionContext));

            Context.SystemService.Execute(new RemoveUserFromRecordTeamRequest()
            {
                Record = target,
                SystemUserId = User.Get(Context.ExecutionContext).Id,
                TeamTemplateId = TeamTemplate.Get(Context.ExecutionContext).Id
            });
        }
    }
}
