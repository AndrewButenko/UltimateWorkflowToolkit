using System.Activities;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.MultiselectHelper
{
    public class GetFieldLabel : MultiselectWorkflowBase
    {
        #region Input/Output Parameters

        [Output("Field Label")]
        public OutArgument<string> FieldLabel { get; set; }

        #endregion
        protected override void ExecuteWorkflowLogic()
        {
            var record = Record;

            var result = string.Empty;

            if (record.Contains(FieldName))
                result = record.FormattedValues[FieldName];

            FieldLabel.Set(Context.ExecutionContext, result);
        }
    }
}
