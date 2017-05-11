using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class OpportunityGenerateInvoice : CrmWorkflowBase
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

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service)
        {
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
