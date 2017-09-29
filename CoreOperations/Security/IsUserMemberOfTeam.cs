using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Security
{
    public class IsUserMemberOfTeam: CrmWorkflowBase
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

        [Output("Is Member")]
        public OutArgument<bool> IsMember { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic()
        {
            var userQuery = new QueryExpression("systemuser")
            {
                ColumnSet = new ColumnSet(false)
            };
            userQuery.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, User.Get(Context.ExecutionContext).Id);

            var teamLink = userQuery.AddLink("teammembership", "systemuserid", "systemuserid");
            teamLink.LinkCriteria.AddCondition("teamid", ConditionOperator.Equal, Team.Get(Context.ExecutionContext).Id);

            var isMember = Context.SystemService.RetrieveMultiple(userQuery).Entities.Count != 0;

            IsMember.Set(Context.ExecutionContext, isMember);
        }
    }
}
