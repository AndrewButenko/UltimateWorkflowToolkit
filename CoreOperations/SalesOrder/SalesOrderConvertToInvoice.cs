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

        #region Overriddes

        protected override EntityReference GetSourceEntity(CodeActivityContext executionContext)
        {
            return SalesOrder.Get(executionContext);
        }

        protected override void SetTargetEntity(CodeActivityContext executionContext, EntityReference target)
        {
            Invoice.Set(executionContext, target);
        }

        protected override string SourceEntityChild => "salesorderdetail";
        protected override string SourceEntityLookupFieldName => "salesorderid";
        protected override string TargetEntity => "invoice";
        protected override string TargetEntityChild => "invoicedetail";
        protected override string TargetEntityLookupFieldName => "invoiceid";

        #endregion Overriddes

    }
}
