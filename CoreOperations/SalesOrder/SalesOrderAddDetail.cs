using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.CoreOperations.Base;

namespace UltimateWorkflowToolkit.CoreOperations.SalesOrder
{
    public class SalesOrderAddDetail : AddDetailWorkflowExtensionBase
    {
        #region Inputs

        [Input("Order")]
        [ReferenceTarget("salesorder")]
        [RequiredArgument]
        public InArgument<EntityReference> SalesOrder { get; set; }

        [Input("Freight Terms")]
        [AttributeTarget("salesorderdetail", "shipto_freighttermscode")]
        public InArgument<OptionSetValue> ShipToFreightTerms { get; set; }

        [Input("Ship To Contact Name")]
        public InArgument<string> ShipToContactName { get; set; }

        [Input("Quantity Back Ordered")]
        public InArgument<decimal> QuantityBackOrdered { get; set; }

        [Input("Quantity Canceled")]
        public InArgument<decimal> QuantityCancelled { get; set; }

        [Input("Quantity Shipped")]
        public InArgument<decimal> QuantityShipped { get; set; }

        [Input("Requested Delivery Date")]
        public InArgument<DateTime> RequestDeliveryBy { get; set; }

        [Input("Copied")]
        [Default("False")]
        public InArgument<bool> IsCopied { get; set; }

        #endregion Inputs

        #region Overriddes

        protected override string ProductEntityName => "salesorderdetail";
        protected override string ParentEntityLookupFieldName => "salesorderid";
        protected override EntityReference ParentEntity => SalesOrder.Get(Context.ExecutionContext);

        protected override void ProcessAdditionalFields(ref Entity record)
        {
            record["shipto_freighttermscode"] = ShipToFreightTerms.Get(Context.ExecutionContext);
            record["shipto_contactname"] = ShipToContactName.Get(Context.ExecutionContext);
            record["quantitybackordered"] = QuantityBackOrdered.Get(Context.ExecutionContext);
            record["quantitycancelled"] = QuantityCancelled.Get(Context.ExecutionContext);
            record["quantityshipped"] = QuantityShipped.Get(Context.ExecutionContext);
            record["requestdeliveryby"] = RequestDeliveryBy.Get(Context.ExecutionContext);
            record["iscopied"] = IsCopied.Get(Context.ExecutionContext);

            base.ProcessAdditionalFields(ref record);
        }

        #endregion Overriddes
    }
}
