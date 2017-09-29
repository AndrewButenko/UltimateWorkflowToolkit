using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.CoreOperations.Base;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class OpportunityGenerateSalesOrder : CreateChildFromParentWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Opportunity")]
        [ReferenceTarget("opportunity")]
        [RequiredArgument]
        public InArgument<EntityReference> Opportunity { get; set; }

        [Output("Sales Order")]
        [ReferenceTarget("salesorder")]
        public OutArgument<EntityReference> SalesOrder { get; set; }

        #endregion Input/Output Parameters

        #region Overriddes

        protected override EntityReference SourceEntity => Opportunity.Get(Context.ExecutionContext);

        protected override void SetTargetEntity(EntityReference target)
        {
            SalesOrder.Set(Context.ExecutionContext, target);
        }

        protected override string SourceEntityChild => "opportunityproduct";
        protected override string SourceEntityLookupFieldName => "opportunityid";
        protected override string TargetEntity => "salesorder";
        protected override string TargetEntityChild => "salesorderdetail";
        protected override string TargetEntityLookupFieldName => "salesorderid";

        #endregion Overriddes

    }
}
