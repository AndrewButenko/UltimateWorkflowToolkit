using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class SalesOrderUnlockPricing : CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Order")]
        [ReferenceTarget("salesorder")]
        [RequiredArgument]
        public InArgument<EntityReference> SalesOrder { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic()
        {
            var unlockSalesOrderPricingRequest = new UnlockSalesOrderPricingRequest()
            {
                SalesOrderId = SalesOrder.Get(Context.ExecutionContext).Id
            };

            Context.UserService.Execute(unlockSalesOrderPricingRequest);
        }

    }
}
