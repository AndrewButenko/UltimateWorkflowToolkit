using System;
using System.Activities;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.DateTimeRoutines
{
    public class FormatDateTime: CrmWorkflowBase
    {
        #region Inputs/Outputs

        [Input("DateTime Value")]
        [RequiredArgument]
        public InArgument<DateTime> DateTimeValue { get; set; }

        [Input("Format")]
        [RequiredArgument]
        public InArgument<string> Format { get; set; }

        [Output("Formatted Value")]
        public OutArgument<string> FormattedDateTimeValue { get; set; }

        #endregion Inputs/Outputs

        protected override void ExecuteWorkflowLogic()
        {
            var formattedValue =
                DateTimeValue.Get(Context.ExecutionContext).ToString(Format.Get(Context.ExecutionContext));

            FormattedDateTimeValue.Set(Context.ExecutionContext, formattedValue);
        }
    }
}
