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

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service)
        {
            var convertQuoteToSalesOrderRequest = new ConvertQuoteToSalesOrderRequest()
            {
                ColumnSet = new ColumnSet("salesorderid"),
                QuoteCloseDate = CloseDate.Get(executionContext),
                QuoteCloseDescription = Description.Get(executionContext),
                QuoteCloseStatus = QuoteStatus.Get(executionContext),
                QuoteCloseSubject = Subject.Get(executionContext),
                QuoteId = Quote.Get(executionContext).Id
            };

            var convertQuoteToSalesOrderResponse =
                (ConvertQuoteToSalesOrderResponse) service.Execute(convertQuoteToSalesOrderRequest);

            SalesOrder.Set(executionContext, convertQuoteToSalesOrderResponse.Entity.ToEntityReference());
        }
    }
}
