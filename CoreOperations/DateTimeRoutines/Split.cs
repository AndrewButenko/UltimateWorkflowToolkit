using System;
using System.Activities;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.DateTimeRoutines
{
    public class Split: CrmWorkflowBase
    {
        #region Inputs/Outputs

        [Input("DateTime Value")]
        [RequiredArgument]
        public InArgument<DateTime> DateTimeValue { get; set; }

        [Output("Year")]
        public OutArgument<int> Year { get; set; }

        [Output("Month")]
        public OutArgument<int> Month { get; set; }

        [Output("Day")]
        public OutArgument<int> Day { get; set; }

        [Output("Day of Week")]
        public  OutArgument<int> DayofWeek { get; set; }

        #endregion Inputs/Outputs

        protected override void ExecuteWorkflowLogic()
        {
            var dateTimeValue = DateTimeValue.Get(Context.ExecutionContext);
            Year.Set(Context.ExecutionContext, dateTimeValue.Year);
            Month.Set(Context.ExecutionContext, dateTimeValue.Month);
            Day.Set(Context.ExecutionContext, dateTimeValue.Day);
            DayofWeek.Set(Context.ExecutionContext, (int)dateTimeValue.DayOfWeek);
        }
    }
}
