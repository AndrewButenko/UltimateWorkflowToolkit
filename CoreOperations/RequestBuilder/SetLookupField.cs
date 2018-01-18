using System.Activities;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.CoreOperations.Base;

namespace UltimateWorkflowToolkit.CoreOperations.RequestBuilder
{
    public class SetLookupField: SetFieldWorkflowBase
    {
        #region Inputs

        [Input("Field Value")]
        public InArgument<string> FieldValue { get; set; }

        #endregion Inputs

        protected override void AddField(ref Dictionary<string, object> dictionary, string fieldName)
        {
            var lookup = ConvertToEntityReference(FieldValue.Get(Context.ExecutionContext));
            dictionary.Add(fieldName, lookup);
        }
    }
}
