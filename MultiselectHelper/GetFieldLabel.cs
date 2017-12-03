using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
