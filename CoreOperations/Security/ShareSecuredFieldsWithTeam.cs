using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.CoreOperations.Security
{
    public class ShareSecuredFieldsWithTeam : ShareSecuredFieldsBase
    {
        #region Input/Output Parameters

        [Input("Team")]
        [RequiredArgument]
        [ReferenceTarget("team")]
        public InArgument<EntityReference> Team { get; set; }

        #endregion Input/Output Parameters

        #region Overrides

        public override EntityReference Principal => Team.Get(Context.ExecutionContext);

        #endregion Overrides
    }
}
