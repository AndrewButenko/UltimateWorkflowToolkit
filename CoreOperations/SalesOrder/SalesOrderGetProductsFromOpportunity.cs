using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.CoreOperations.Base;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class SalesOrderGetProductsFromOpportunity : CopyDetailsWorkflowsBase
    {

        #region Input/Output Parameters

        [Input("Order")]
        [ReferenceTarget("salesorder")]
        [RequiredArgument]
        public InArgument<EntityReference> SalesOrder { get; set; }

        [Input("Opportunity")]
        [ReferenceTarget("opportunity")]
        [RequiredArgument]
        public InArgument<EntityReference> Opportunity { get; set; }

        #endregion Input/Output Parameters

        #region Overriddes

        protected override EntityReference GetSourceEntityParent(CodeActivityContext executionContext)
        {
            return Opportunity.Get(executionContext);
        }

        protected override EntityReference GetTargetEntityParent(CodeActivityContext executionContext)
        {
            return SalesOrder.Get(executionContext);
        }

        protected override string SourceEntity => "opportunityproduct";
        protected override string SourceEntityLookupFieldName => "opportunityid";
        protected override string TargetEntity => "salesorderdetail";
        protected override string TargetEntityLookupFieldName => "salesorderid";

        #endregion Overriddes

    }
}
