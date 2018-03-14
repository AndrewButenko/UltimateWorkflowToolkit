using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.CoreOperations.Base;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class SalesOrderConvertToInvoice: CreateChildFromParentWorkflowBase
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

        protected override EntityReference SourceEntity => SalesOrder.Get(Context.ExecutionContext);

        protected override void SetTargetEntity(EntityReference target)
        {
            Invoice.Set(Context.ExecutionContext, target);
        }

        protected override string SourceEntityChild => "salesorderdetail";
        protected override string SourceEntityLookupFieldName => "salesorderid";
        protected override string TargetEntity => "invoice";
        protected override string TargetEntityChild => "invoicedetail";
        protected override string TargetEntityLookupFieldName => "invoiceid";
    }
}
