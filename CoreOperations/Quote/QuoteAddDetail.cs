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

        #endregion Input Arguments

        #region Overriddes

        protected override string ProductEntityName => "quotedetail";
        protected override string ParentEntityLookupFieldName => "quoteid";
        protected override EntityReference GetParentEntity(CodeActivityContext executionContext)
        {
            return Quote.Get(executionContext);
        }

        protected override void ProcessAdditionalFields(ref Entity record, CodeActivityContext executionContext)
        {
            record["shipto_freighttermscode"] = ShipToFreightTerms.Get(executionContext);
            record["requestdeliveryby"] = RequestDeliveryBy.Get(executionContext);

            base.ProcessAdditionalFields(ref record, executionContext);
        }

        #endregion Overriddes

    }
}
