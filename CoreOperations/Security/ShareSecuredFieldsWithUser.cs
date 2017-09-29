using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.CoreOperations.Security
{
    public class ShareSecuredFieldsWithUser : ShareSecuredFieldsBase
    {
        #region Input/Output Parameters

        [Input("User")]
        [RequiredArgument]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> User { get; set; }

        #endregion Input/Output Parameters

        #region Overrides

        public override EntityReference Principal => User.Get(Context.ExecutionContext);

        #endregion Overrides
    }
}
