using System.Activities;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.CoreOperations.Base;

namespace UltimateWorkflowToolkit.CoreOperations.RequestBuilder
{
    public class SetOptionSetField: SetFieldWorkflowBase
    {
        #region Inputs

        [Input("Field Value")]
        public InArgument<int> FieldValue { get; set; }

        #endregion Inputs

        protected override void AddField(ref Dictionary<string, object> dictionary, string fieldName)
        {
            dictionary.Add(fieldName, new OptionSetValue(FieldValue.Get(Context.ExecutionContext)));
        }
    }
}
