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

        public override EntityReference Principal
        {
            get
            {
                if (Context == null || Context.ExecutionContext == null)
                {
                    return null;
                }

                return User.Get(Context.ExecutionContext);
            }
        }
    }
}
