using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class SalesOrderConvertToInvoice: CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Order")]
        [ReferenceTarget("salesorder")]
        [RequiredArgument]
        public InArgument<EntityReference> SalesOrder { get; set; }

        [Output("Invoice")]
        [ReferenceTarget("invoice")]
        public OutArgument<EntityReference> Invoice { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service, IOrganizationService sysService)
        {
            var convertSalesOrderToInvoiceRequest = new ConvertSalesOrderToInvoiceRequest
            {
                SalesOrderId = SalesOrder.Get(executionContext).Id,
                ColumnSet = new ColumnSet("invoiceid")
            };

            var convertSalesOrderToInvoiceResponse =
                (ConvertSalesOrderToInvoiceResponse)service.Execute(convertSalesOrderToInvoiceRequest);

            Invoice.Set(executionContext, convertSalesOrderToInvoiceResponse.Entity.ToEntityReference());
        }
    }
}
