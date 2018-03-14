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

        protected override void ExecuteWorkflowLogic()
        {
            var emailId = Email.Get(Context.ExecutionContext).Id;

            var email = Context.UserService.Retrieve("email", emailId, new ColumnSet("to"));
            var to = email.Contains("to") ? email.GetAttributeValue<EntityCollection>("to") : new EntityCollection();

            var userQuery = new QueryExpression("systemuser")
            {
                ColumnSet = new ColumnSet(false)
            };
            var teamLink = userQuery.AddLink("teammembership", "systemuserid", "systemuserid");
            teamLink.LinkCriteria.AddCondition("teamid", ConditionOperator.Equal, Team.Get(Context.ExecutionContext).Id);

            var users = QueryWithPaging(userQuery);

            users.ForEach(u =>
            {
                to.Entities.Add(new Entity("activityparty")
                {
                    ["partyid"] = u.ToEntityReference()
                });
            });

            email["to"] = to;
            Context.UserService.Update(email);

            if (SendAfterOperation.Get(Context.ExecutionContext))
                base.ExecuteWorkflowLogic();
        }
    }
}
