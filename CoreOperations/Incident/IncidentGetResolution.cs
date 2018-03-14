using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.CoreOperations.Base;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class IncidentGetResolution: GetResolutionWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Case")]
        [ReferenceTarget("incident")]
        [RequiredArgument]
        public InArgument<EntityReference> Case { get; set; }

        [Output("Case Resolution")]
        [ReferenceTarget("incidentresolution")]
        public OutArgument<EntityReference> CaseResolution { get; set; }

        #endregion Input/Output Parameters

        protected override Guid ParentRecordId => Case.Get(Context.ExecutionContext).Id;

        protected override string ResolutionEntityName => "incidentresolution";

        protected override string ParentRecordLookupFieldName => "incidentid";

        protected override void SetResolutionEntity(EntityReference resolution)
        {
            CaseResolution.Set(Context.ExecutionContext, resolution);
        }
    }
}
