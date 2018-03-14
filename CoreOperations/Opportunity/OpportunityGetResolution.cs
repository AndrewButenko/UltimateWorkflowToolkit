using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.CoreOperations.Base;

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

        protected override Guid ParentRecordId => Opportunity.Get(Context.ExecutionContext).Id;

        protected override string ResolutionEntityName => "opportunityclose";

        protected override string ParentRecordLookupFieldName => "opportunityid";

        protected override void SetResolutionEntity(EntityReference resolution)
        {
            OpportunityClose.Set(Context.ExecutionContext, resolution);
        }
    }
}
