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

        protected override void ExecuteWorkflowLogic()
        {
            var reviseQuoteRequest = new ReviseQuoteRequest()
            {
                ColumnSet = new ColumnSet("quoteid"),
                QuoteId = Quote.Get(Context.ExecutionContext).Id
            };

            var reviseQuoteResponse = (ReviseQuoteResponse) Context.UserService.Execute(reviseQuoteRequest);

            RevisedQuote.Set(Context.ExecutionContext, reviseQuoteResponse.Entity.ToEntityReference());
        }
    }
}
