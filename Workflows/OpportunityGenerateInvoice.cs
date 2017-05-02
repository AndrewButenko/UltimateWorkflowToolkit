using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class OpportunityGenerateInvoice : CodeActivity
    {
        #region Input/Output Parameters

        [Input("Opportunity")]
        [ReferenceTarget("opportunity")]
        [RequiredArgument]
        public InArgument<EntityReference> Opportunity { get; set; }

        [Output("Invoice")]
        [ReferenceTarget("invoice")]
        public OutArgument<EntityReference> Invoice { get; set; }

        #endregion Input/Output Parameters

        protected override void Execute(CodeActivityContext executionContext)
        {
            var context = executionContext.GetExtension<IWorkflowContext>();
            var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            var service = serviceFactory.CreateOrganizationService(context.UserId);

            var generateInvoiceFromOpportunityRequest = new GenerateInvoiceFromOpportunityRequest
            {
                OpportunityId = Opportunity.Get(executionContext).Id,
                ColumnSet = new ColumnSet("invoiceid")
            };

            var generateInvoiceFromOpportunityResponse =
                (GenerateInvoiceFromOpportunityResponse)service.Execute(generateInvoiceFromOpportunityRequest);

            Invoice.Set(executionContext, generateInvoiceFromOpportunityResponse.Entity.ToEntityReference());
        }
    }
}
