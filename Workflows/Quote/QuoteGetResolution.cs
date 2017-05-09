using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class QuoteGetResolution: GetResolutionWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Quote")]
        [ReferenceTarget("quote")]
        [RequiredArgument]
        public InArgument<EntityReference> Quote { get; set; }

        [Output("Quote Close")]
        [ReferenceTarget("quoteclose")]
        public OutArgument<EntityReference> QuoteClose { get; set; }

        #endregion Input/Output Parameters

        #region Overrides

        protected override Guid GetParentRecordId(CodeActivityContext executionContext)
        {
            return Quote.Get(executionContext).Id;
        }

        protected override string ResolutionEntityName => "quoteclose";

        protected override string ParentRecordLookupFieldName => "quoteid";

        protected override void SetResolutionEntity(CodeActivityContext executionContext, EntityReference resolution)
        {
            QuoteClose.Set(executionContext, resolution);
        }

        #endregion Overrides
    }
}
