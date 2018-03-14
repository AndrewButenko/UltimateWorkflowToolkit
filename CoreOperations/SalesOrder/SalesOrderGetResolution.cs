using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.CoreOperations.Base;

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

        protected override Guid ParentRecordId => Order.Get(Context.ExecutionContext).Id;

        protected override string ResolutionEntityName => "orderclose";

        protected override string ParentRecordLookupFieldName => "salesorderid";

        protected override void SetResolutionEntity(EntityReference resolution)
        {
            OrderClose.Set(Context.ExecutionContext, resolution);
        }
    }
}
