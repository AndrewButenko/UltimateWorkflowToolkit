using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class QuoteConvertToSalesOrder: CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Quote")]
        [ReferenceTarget("quote")]
        [RequiredArgument]
        public InArgument<EntityReference> Quote { get; set; }

        [Input("Quote Status")]
        [AttributeTarget("quote", "statuscode")]
        [RequiredArgument]
        public InArgument<OptionSetValue> QuoteStatus { get; set; }

        [Input("Quote Close: Subject")]
        public InArgument<string> Subject { get; set; }

        [Input("Quote Close: Close Date")]
        [RequiredArgument]
        public InArgument<DateTime> CloseDate { get; set; }

        [Input("Quote Close: Description")]
        public InArgument<string> Description { get; set; }

        [Output("Sales Order")]
        [ReferenceTarget("salesorder")]
        public OutArgument<EntityReference> SalesOrder { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic()
        {
            var convertQuoteToSalesOrderRequest = new ConvertQuoteToSalesOrderRequest()
            {
                ColumnSet = new ColumnSet("salesorderid"),
                QuoteCloseDate = CloseDate.Get(Context.ExecutionContext),
                QuoteCloseDescription = Description.Get(Context.ExecutionContext),
                QuoteCloseStatus = QuoteStatus.Get(Context.ExecutionContext),
                QuoteCloseSubject = Subject.Get(Context.ExecutionContext),
                QuoteId = Quote.Get(Context.ExecutionContext).Id
            };

            var convertQuoteToSalesOrderResponse =
                (ConvertQuoteToSalesOrderResponse) Context.UserService.Execute(convertQuoteToSalesOrderRequest);

            SalesOrder.Set(Context.ExecutionContext, convertQuoteToSalesOrderResponse.Entity.ToEntityReference());
        }
    }
}
