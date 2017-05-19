using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class SalesOrderGetProductsFromOpportunity : CrmWorkflowBase
    {

        #region Input/Output Parameters

        [Input("Order")]
        [ReferenceTarget("salesorder")]
        [RequiredArgument]
        public InArgument<EntityReference> SalesOrder { get; set; }

        [Input("Opportunity")]
        [ReferenceTarget("opportunity")]
        [RequiredArgument]
        public InArgument<EntityReference> Opportunity { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service, IOrganizationService sysService)
        {
            var getSalesOrderProductsFromOpportunityRequest = new GetSalesOrderProductsFromOpportunityRequest()
            {
                SalesOrderId = SalesOrder.Get(executionContext).Id,
                OpportunityId = Opportunity.Get(executionContext).Id
            };

            service.Execute(getSalesOrderProductsFromOpportunityRequest);
        }
    }
}
