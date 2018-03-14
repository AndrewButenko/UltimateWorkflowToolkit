using System.Activities;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.CoreOperations.Base
{
    public abstract class SetFieldWorkflowBase: BuildRequestWorkflowBase
    {
        #region Inputs

        [Input("Field Name")]
        [RequiredArgument]
        public InArgument<string> FieldName { get; set; }

        #endregion Inputs

        protected abstract void AddField(ref Dictionary<string, object> request, string fieldName);

        protected override void BuildRequest(ref Dictionary<string, object> request)
        {
            var fieldName = FieldName.Get(Context.ExecutionContext);
            AddField(ref request, fieldName);
        }
    }
}
