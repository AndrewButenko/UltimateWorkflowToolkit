using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.CoreOperations.Base;

namespace UltimateWorkflowToolkit.CoreOperations.Quote
{
    public class QuoteAddDetail : AddDetailWorkflowExtensionBase
    {
        #region Input Arguments

        [Input("Quote")]
        [ReferenceTarget("quote")]
        [RequiredArgument]
        public InArgument<EntityReference> Quote { get; set; }

        [Input("Freight Terms")]
        [AttributeTarget("quotedetail", "shipto_freighttermscode")]
        public InArgument<OptionSetValue> ShipToFreightTerms { get; set; }

        [Input("Requested Delivery Date")]
        public InArgument<DateTime> RequestDeliveryBy { get; set; }

        [Input("Ship To Contact Name")]
        public InArgument<string> ShipToContactName { get; set; }

        #endregion Input Arguments

        #region Overriddes

        protected override string ProductEntityName => "quotedetail";
        protected override string ParentEntityLookupFieldName => "quoteid";
        protected override EntityReference ParentEntity => Quote.Get(Context.ExecutionContext);

        protected override void ProcessAdditionalFields(ref Entity record)
        {
            record["shipto_freighttermscode"] = ShipToFreightTerms.Get(Context.ExecutionContext);
            record["requestdeliveryby"] = RequestDeliveryBy.Get(Context.ExecutionContext);
            record["shipto_contactname"] = ShipToContactName.Get(Context.ExecutionContext);

            base.ProcessAdditionalFields(ref record);
        }

        #endregion Overriddes

    }
}
