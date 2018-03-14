using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.CoreOperations.Base;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class InvoiceGetProductsFromOpportunity : CopyDetailsWorkflowsBase
    {

        #region Input/Output Parameters

        [Input("Invoice")]
        [ReferenceTarget("invoice")]
        [RequiredArgument]
        public InArgument<EntityReference> Invoice { get; set; }

        [Input("Opportunity")]
        [ReferenceTarget("opportunity")]
        [RequiredArgument]
        public InArgument<EntityReference> Opportunity { get; set; }

        #endregion Input/Output Parameters

        protected override EntityReference SourceEntityParent => Opportunity.Get(Context.ExecutionContext);

        protected override EntityReference TargetEntityParent => Invoice.Get(Context.ExecutionContext);

        protected override string SourceEntity => "opportunityproduct";
        protected override string SourceEntityLookupFieldName => "opportunityid";
        protected override string TargetEntity => "invoicedetail";
        protected override string TargetEntityLookupFieldName => "invoiceid";
    }
}
