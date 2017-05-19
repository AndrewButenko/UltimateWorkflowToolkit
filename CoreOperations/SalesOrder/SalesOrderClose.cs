using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class SalesOrderClose : CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Order")]
        [ReferenceTarget("salesorder")]
        [RequiredArgument]
        public InArgument<EntityReference> SalesOrder { get; set; }

        [Input("Order Status")]
        [AttributeTarget("salesorder", "statuscode")]
        [RequiredArgument]
        public InArgument<OptionSetValue> SalesOrderStatus { get; set; }

        [Input("Order Close: Subject")]
        public InArgument<string> Subject { get; set; }

        [Input("Order Close: Close Date")]
        [RequiredArgument]
        public InArgument<DateTime> CloseDate { get; set; }

        [Input("Order Close: Description")]
        public InArgument<string> Description { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service, IOrganizationService sysService)
        {
            var cancelSalesOrderRequest = new CancelSalesOrderRequest()
            {
                Status = SalesOrderStatus.Get(executionContext),
                OrderClose = new Entity("orderclose")
                {
                    ["subject"] = Subject.Get(executionContext),
                    ["salesorderid"] = SalesOrder.Get(executionContext),
                    ["actualend"] = CloseDate.Get(executionContext),
                    ["description"] = Description.Get(executionContext)
                }
            };

            service.Execute(cancelSalesOrderRequest);
        }
    }
}
