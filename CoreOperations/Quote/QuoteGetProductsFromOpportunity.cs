using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class QuoteGetProductsFromOpportunity: CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Quote")]
        [ReferenceTarget("quote")]
        [RequiredArgument]
        public InArgument<EntityReference> Quote { get; set; }

        [Input("Opportunity")]
        [ReferenceTarget("opportunity")]
        [RequiredArgument]
        public InArgument<EntityReference> Opportunity { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service)
        {
            var getQuoteProductsFromOpportunityRequest = new GetQuoteProductsFromOpportunityRequest()
            {
                QuoteId = Quote.Get(executionContext).Id,
                OpportunityId = Opportunity.Get(executionContext).Id
            };

            service.Execute(getQuoteProductsFromOpportunityRequest);
        }
    }
}
