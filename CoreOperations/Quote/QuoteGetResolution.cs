using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.CoreOperations.Base;

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

        protected override Guid ParentRecordId => Quote.Get(Context.ExecutionContext).Id;

        protected override string ResolutionEntityName => "quoteclose";

        protected override string ParentRecordLookupFieldName => "quoteid";

        protected override void SetResolutionEntity(EntityReference resolution)
        {
            QuoteClose.Set(Context.ExecutionContext, resolution);
        }
    }
}
