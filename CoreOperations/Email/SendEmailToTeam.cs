using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.CoreOperations.Email
{
    public class SendEmailToTeam : SendEmail
    {
        #region Inputs/Outputs

        [Input("Team")]
        [RequiredArgument]
        [ReferenceTarget("team")]
        public InArgument<EntityReference> Team { get; set; }

        [Input("Send Email")]
        [RequiredArgument]
        [Default("True")]
        public InArgument<bool> SendAfterOperation { get; set; }

        #endregion Inputs/Outputs

        #region Overriddes

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service,
            IOrganizationService sysService)
        {
            var emailId = Email.Get(executionContext).Id;

            var email = sysService.Retrieve("email", emailId, new ColumnSet("to"));
            var to = email.Contains("to") ? email.GetAttributeValue<EntityCollection>("to") : new EntityCollection();

            var userQuery = new QueryExpression("systemuser")
            {
                ColumnSet = new ColumnSet(false)
            };
            var teamLink = userQuery.AddLink("teammembership", "systemuserid", "systemuserid");
            teamLink.LinkCriteria.AddCondition("teamid", ConditionOperator.Equal, Team.Get(executionContext).Id);

            var users = QueryWithPaging(userQuery, service);

            users.ForEach(u =>
            {
                to.Entities.Add(new Entity("activityparty")
                {
                    ["partyid"] = u.ToEntityReference()
                });
            });

            email["to"] = to;
            sysService.Update(email);

            if (SendAfterOperation.Get(executionContext))
                base.ExecuteWorkflowLogic(executionContext, context, service, sysService);
        }

        #endregion Overriddes

    }
}
