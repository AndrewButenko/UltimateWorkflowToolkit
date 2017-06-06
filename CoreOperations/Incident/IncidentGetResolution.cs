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

        #region Overrides

        protected override Guid GetParentRecordId(CodeActivityContext executionContext)
        {
            return Case.Get(executionContext).Id;
        }

        protected override string ResolutionEntityName => "incidentresolution";

        protected override string ParentRecordLookupFieldName => "incidentid";

        protected override void SetResolutionEntity(CodeActivityContext executionContext, EntityReference resolution)
        {
            CaseResolution.Set(executionContext, resolution);
        }

        #endregion Overrides
    }
}
