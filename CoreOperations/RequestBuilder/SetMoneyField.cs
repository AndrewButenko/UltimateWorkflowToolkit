using System.Activities;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.CoreOperations.Base;

namespace UltimateWorkflowToolkit.CoreOperations.RequestBuilder
{
    public class SetMoneyField: SetFieldWorkflowBase
    {
        #region Inputs

        [Input("Field Value")]
        public InArgument<Money> FieldValue { get; set; }

        #endregion Inputs

        protected override void AddField(ref Dictionary<string, object> dictionary, string fieldName)
        {
            dictionary.Add(fieldName, FieldValue.Get(Context.ExecutionContext));
        }
    }
}
