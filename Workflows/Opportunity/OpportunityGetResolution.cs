using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class OpportunityGetResolution: GetResolutionWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Opportunity")]
        [ReferenceTarget("opportunity")]
        [RequiredArgument]
        public InArgument<EntityReference> Opportunity { get; set; }

        [Output("Opportunity Close")]
        [ReferenceTarget("opportunityclose")]
        public OutArgument<EntityReference> OpportunityClose { get; set; }

        #endregion Input/Output Parameters

        #region Overrides

        protected override Guid GetParentRecordId(CodeActivityContext executionContext)
        {
            return Opportunity.Get(executionContext).Id;
        }

        protected override string ResolutionEntityName => "opportunityclose";

        protected override string ParentRecordLookupFieldName => "opportunityid";

        protected override void SetResolutionEntity(CodeActivityContext executionContext, EntityReference resolution)
        {
            OpportunityClose.Set(executionContext, resolution);
        }

        #endregion Overrides
    }
}
