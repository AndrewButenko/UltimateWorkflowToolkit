using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class SalesOrderGetResolution: GetResolutionWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Order")]
        [ReferenceTarget("salesorder")]
        [RequiredArgument]
        public InArgument<EntityReference> Order { get; set; }

        [Output("Order Close")]
        [ReferenceTarget("orderclose")]
        public OutArgument<EntityReference> OrderClose { get; set; }

        #endregion Input/Output Parameters

        #region Overrides

        protected override Guid GetParentRecordId(CodeActivityContext executionContext)
        {
            return Order.Get(executionContext).Id;
        }

        protected override string ResolutionEntityName => "orderclose";

        protected override string ParentRecordLookupFieldName => "salesorderid";

        protected override void SetResolutionEntity(CodeActivityContext executionContext, EntityReference resolution)
        {
            OrderClose.Set(executionContext, resolution);
        }

        #endregion Overrides
    }
}
