using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using UltimateWorkflowToolkit.CoreOperations.Base;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class OpportunityGenerateQuote : CreateChildFromParentWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Opportunity")]
        [ReferenceTarget("opportunity")]
        [RequiredArgument]
        public InArgument<EntityReference> Opportunity { get; set; }

        [Output("Quote")]
        [ReferenceTarget("quote")]
        public OutArgument<EntityReference> Quote { get; set; }

        #endregion Input/Output Parameters

        #region Overriddes

        protected override EntityReference GetSourceEntity(CodeActivityContext executionContext)
        {
            return Opportunity.Get(executionContext);
        }

        protected override void SetTargetEntity(CodeActivityContext executionContext, EntityReference target)
        {
            Quote.Set(executionContext, target);
        }

        protected override string SourceEntityChild => "opportunityproduct";
        protected override string SourceEntityLookupFieldName => "opportunityid";
        protected override string TargetEntity => "quote";
        protected override string TargetEntityChild => "quotedetail";
        protected override string TargetEntityLookupFieldName => "quoteid";

        #endregion Overriddes

    }
}
