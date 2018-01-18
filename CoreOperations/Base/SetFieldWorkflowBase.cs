using System.Activities;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Base
{
    public abstract class SetFieldWorkflowBase: BuildRequestWorkflowBase
    {
        #region Inputs

        [Input("Field Name")]
        [RequiredArgument]
        public InArgument<string> FieldName { get; set; }

        #endregion Inputs

        #region Abstracts

        protected abstract void AddField(ref Dictionary<string, object> request, string fieldName);

        #endregion Abstracts

        #region Overrides

        protected override void BuildRequest(ref Dictionary<string, object> request)
        {
            var fieldName = FieldName.Get(Context.ExecutionContext);
            AddField(ref request, fieldName);
        }

        #endregion Overrides
    }
}
