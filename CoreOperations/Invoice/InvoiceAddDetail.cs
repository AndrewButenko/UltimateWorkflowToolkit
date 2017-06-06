using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.CoreOperations.Base;

namespace UltimateWorkflowToolkit.CoreOperations.Invoice
{
    public class InvoiceAddDetail : AddDetailWorkflowExtensionBase
    {
        #region Inputs

        [Input("Invoice")]
        [ReferenceTarget("invoice")]
        [RequiredArgument]
        public InArgument<EntityReference> Invoice { get; set; }

        [Input("Delievered On")]
        public InArgument<DateTime> ActualDeliveryOn { get; set; }

        [Input("Copied")]
        [Default("False")]
        public InArgument<bool> IsCopied { get; set; }

        [Input("Quantity Back Ordered")]
        public InArgument<decimal> QuantityBackOrdered { get; set; }

        [Input("Quantity Canceled")]
        public InArgument<decimal> QuantityCancelled { get; set; }

        [Input("Quantity Shipped")]
        public InArgument<decimal> QuantityShipped { get; set; }

        [Input("Shipment Tracking Number")]
        public InArgument<string> ShippingTrackingNumber { get; set; }

        #endregion Inputs

        #region Overriddes

        protected override string ProductEntityName => "invoicedetail";
        protected override string ParentEntityLookupFieldName => "invoiceid";
        protected override EntityReference GetParentEntity(CodeActivityContext executionContext)
        {
            return Invoice.Get(executionContext);
        }

        protected override void ProcessAdditionalFields(ref Entity record, CodeActivityContext executionContext)
        {
            record["actualdeliveryon"] = ActualDeliveryOn.Get(executionContext);
            record["iscopied"] = IsCopied.Get(executionContext);
            record["quantitybackordered"] = QuantityBackOrdered.Get(executionContext);
            record["quantitycancelled"] = QuantityCancelled.Get(executionContext);
            record["quantityshipped"] = QuantityShipped.Get(executionContext);
            record["shippingtrackingnumber"] = ShippingTrackingNumber.Get(executionContext);

            base.ProcessAdditionalFields(ref record, executionContext);
        }

        #endregion Overriddes

    }
}
