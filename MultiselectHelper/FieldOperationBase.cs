using System.Activities;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.MultiselectHelper
{
    public abstract class FieldOperationBase : MultiselectWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Values")]
        [RequiredArgument]
        public InArgument<string> ValuesString { get; set; }

        protected string Values
        {
            get
            {
                if (Context == null || Context.ExecutionContext == null)
                    return null;

                return ValuesString.Get(Context.ExecutionContext);
            }
        }

        #endregion Input/Output Parameters
    }

}
