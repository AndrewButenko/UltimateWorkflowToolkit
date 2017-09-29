using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.CoreOperations.Base;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class OpportunityGenerateInvoice : CreateChildFromParentWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Opportunity")]
        [ReferenceTarget("opportunity")]
        [RequiredArgument]
        public InArgument<EntityReference> Opportunity { get; set; }

        [Output("Invoice")]
        [ReferenceTarget("invoice")]
        public OutArgument<EntityReference> Invoice { get; set; }

        #endregion Input/Output Parameters

        #region Overriddes

        protected override EntityReference SourceEntity => Opportunity.Get(Context.ExecutionContext);

        protected override void SetTargetEntity(EntityReference target)
        {
            Invoice.Set(Context.ExecutionContext, target);
        }

        protected override string SourceEntityChild => "opportunityproduct";
        protected override string SourceEntityLookupFieldName => "opportunityid";
        protected override string TargetEntity => "invoice";
        protected override string TargetEntityChild => "invoicedetail";
        protected override string TargetEntityLookupFieldName => "invoiceid";

        #endregion Overriddes
    }
}
