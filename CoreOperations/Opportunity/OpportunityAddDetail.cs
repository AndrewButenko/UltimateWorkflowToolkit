using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.CoreOperations.Base;

namespace UltimateWorkflowToolkit.CoreOperations.Opportunity
{
    public class OpportunityAddDetail : AddDetailWorkflowBase
    {
        #region Input Arguments

        [Input("Opportunity")]
        [ReferenceTarget("opportunity")]
        [RequiredArgument]
        public InArgument<EntityReference> Opportunity { get; set; }

        #endregion Input Arguments

        #region Overriddes

        protected override string ProductEntityName => "opportunityproduct";
        protected override string ParentEntityLookupFieldName => "opportunityid";
        protected override EntityReference ParentEntity => Opportunity.Get(Context.ExecutionContext);

        protected override void ProcessAdditionalFields(ref Entity record)
        {
        }

        #endregion Overriddes
    }
}
