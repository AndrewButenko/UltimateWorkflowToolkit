using System.Activities;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.NtoN
{
    public class RemoveMemberFromAllMarketingLists : CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Record Reference")]
        [RequiredArgument]
        public InArgument<string> Record { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic()
        {
            var record = ConvertToEntityReference(Record.Get(Context.ExecutionContext));

            var listsQuery = new QueryExpression("list")
            {
                ColumnSet = new ColumnSet(false)
            };
            var listMemberLink = listsQuery.AddLink("listmember", "listid", "listid");
            listMemberLink.LinkCriteria.AddCondition("entityid", ConditionOperator.Equal, record.Id);

            var allLists = QueryWithPaging(listsQuery);

            allLists.ForEach(l => Context.UserService.Execute(new RemoveMemberListRequest()
            {
                EntityId = record.Id,
                ListId = l.Id
            }));
        }
    }
}
