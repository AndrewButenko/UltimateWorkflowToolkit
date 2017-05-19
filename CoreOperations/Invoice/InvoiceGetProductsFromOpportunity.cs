using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class InvoiceGetProductsFromOpportunity : CrmWorkflowBase
    {

        #region Input/Output Parameters

        [Input("Invoice")]
        [ReferenceTarget("invoice")]
        [RequiredArgument]
        public InArgument<EntityReference> Invoice { get; set; }

        [Input("Opportunity")]
        [ReferenceTarget("opportunity")]
        [RequiredArgument]
        public InArgument<EntityReference> Opportunity { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service, IOrganizationService sysService)
        {
            var getInvoiceProductsFromOpportunityRequest = new GetInvoiceProductsFromOpportunityRequest()
            {
                InvoiceId = Invoice.Get(executionContext).Id,
                OpportunityId = Opportunity.Get(executionContext).Id
            };

            service.Execute(getInvoiceProductsFromOpportunityRequest);
        }
    }
}
