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

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service)
        {
            var record = ConvertToEntityReference(Record.Get(executionContext), service);

            service.Execute(new AddMemberListRequest()
            {
                EntityId = record.Id,
                ListId = List.Get(executionContext).Id
            });
        }
    }
}
