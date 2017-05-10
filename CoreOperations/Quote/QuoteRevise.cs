using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class QuoteRevise: CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Quote")]
        [ReferenceTarget("quote")]
        [RequiredArgument]
        public InArgument<EntityReference> Quote { get; set; }

        [Output("Revised Quote")]
        [ReferenceTarget("quote")]
        public OutArgument<EntityReference> RevisedQuote { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service)
        {
            var reviseQuoteRequest = new ReviseQuoteRequest()
            {
                ColumnSet = new ColumnSet("quoteid"),
                QuoteId = Quote.Get(executionContext).Id
            };

            var reviseQuoteResponse = (ReviseQuoteResponse) service.Execute(reviseQuoteRequest);

            RevisedQuote.Set(executionContext, reviseQuoteResponse.Entity.ToEntityReference());
        }
    }
}
