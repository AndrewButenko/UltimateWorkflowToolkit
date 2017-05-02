using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class OpportunityGenerateQuote : CodeActivity
    {
        #region Input/Output Parameters

        [Input("Opportunity")]
        [ReferenceTarget("opportunity")]
        [RequiredArgument]
        public InArgument<EntityReference> Opportunity { get; set; }

        [Output("Quote")]
        [ReferenceTarget("quote")]
        public OutArgument<EntityReference> Quote { get; set; }

        #endregion Input/Output Parameters

        protected override void Execute(CodeActivityContext executionContext)
        {
            var context = executionContext.GetExtension<IWorkflowContext>();
            var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            var service = serviceFactory.CreateOrganizationService(context.UserId);

            var generateQuoteFromOpportunityRequest = new GenerateQuoteFromOpportunityRequest
            {
                OpportunityId = Opportunity.Get(executionContext).Id,
                ColumnSet = new ColumnSet("quoteid")
            };

            var generateQuoteFromOpportunityResponse =
                (GenerateQuoteFromOpportunityResponse) service.Execute(generateQuoteFromOpportunityRequest);

            Quote.Set(executionContext, generateQuoteFromOpportunityResponse.Entity.ToEntityReference());
        }
    }
}
