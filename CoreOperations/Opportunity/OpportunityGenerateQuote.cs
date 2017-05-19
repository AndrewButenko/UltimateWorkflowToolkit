using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class OpportunityGenerateQuote : CrmWorkflowBase
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

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service, IOrganizationService sysService)
        {
            var generateQuoteFromOpportunityRequest = new GenerateQuoteFromOpportunityRequest
            {
                OpportunityId = Opportunity.Get(executionContext).Id,
                ColumnSet = new ColumnSet("quoteid")
            };

            var generateQuoteFromOpportunityResponse =
                (GenerateQuoteFromOpportunityResponse)service.Execute(generateQuoteFromOpportunityRequest);

            Quote.Set(executionContext, generateQuoteFromOpportunityResponse.Entity.ToEntityReference());
        }
    }
}
