using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.NtoN
{
    public class AddMemberToMarketingList : CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Record Reference")]
        [RequiredArgument]
        public InArgument<string> Record { get; set; }

        [Input("Marketing List")]
        [RequiredArgument]
        [ReferenceTarget("list")]
        public InArgument<EntityReference> List { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic()
        {
            var record = ConvertToEntityReference(Record.Get(Context.ExecutionContext));

            Context.UserService.Execute(new AddMemberListRequest()
            {
                EntityId = record.Id,
                ListId = List.Get(Context.ExecutionContext).Id
            });
        }
    }
}
