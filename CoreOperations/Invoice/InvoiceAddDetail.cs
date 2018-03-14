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

        [Input("Freight Terms")]
        [AttributeTarget("invoicedetail", "shipto_freighttermscode")]
        public InArgument<OptionSetValue> ShipToFreightTerms { get; set; }

        [Input("Quantity Back Ordered")]
        public InArgument<decimal> QuantityBackOrdered { get; set; }

        [Input("Quantity Canceled")]
        public InArgument<decimal> QuantityCancelled { get; set; }

        [Input("Quantity Shipped")]
        public InArgument<decimal> QuantityShipped { get; set; }

        [Input("Shipment Tracking Number")]
        public InArgument<string> ShippingTrackingNumber { get; set; }

        #endregion Inputs

        protected override string ProductEntityName => "invoicedetail";
        protected override string ParentEntityLookupFieldName => "invoiceid";
        protected override EntityReference ParentEntity => Invoice.Get(Context.ExecutionContext);

        protected override void ProcessAdditionalFields(ref Entity record)
        {
            record["actualdeliveryon"] = ActualDeliveryOn.Get(Context.ExecutionContext);
            record["iscopied"] = IsCopied.Get(Context.ExecutionContext);
            record["shipto_freighttermscode"] = ShipToFreightTerms.Get(Context.ExecutionContext);
            record["quantitybackordered"] = QuantityBackOrdered.Get(Context.ExecutionContext);
            record["quantitycancelled"] = QuantityCancelled.Get(Context.ExecutionContext);
            record["quantityshipped"] = QuantityShipped.Get(Context.ExecutionContext);
            record["shippingtrackingnumber"] = ShippingTrackingNumber.Get(Context.ExecutionContext);

            base.ProcessAdditionalFields(ref record);
        }
    }
}
