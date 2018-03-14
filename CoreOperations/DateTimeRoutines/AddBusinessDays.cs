using System;
using System.Linq;
using System.Activities;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.DateTimeRoutines
{
    public class AddBusinessDays : CrmWorkflowBase
    {
        #region Inputs/Outputs

        [Input("DateTime Value")]
        [RequiredArgument]
        public InArgument<DateTime> DateTimeValue { get; set; }

        [Input("Days to Add")]
        [RequiredArgument]
        public InArgument<int> DaystoAdd { get; set; }

        [Input("Weekend Days")]
        [RequiredArgument]
        [Default("0|6")]
        public InArgument<string> WeekendDays { get; set; }

        [Input("Holidays Query (FetchXml)")]
        public InArgument<string> HolidaysQuery { get; set; }

        [Output("Resulting DateTime")]
        public OutArgument<DateTime> Result { get; set; }

        #endregion Inputs/Outputs

        protected override void ExecuteWorkflowLogic()
        {
            var result = DateTimeValue.Get(Context.ExecutionContext);
            var daysToAdd = DaystoAdd.Get(Context.ExecutionContext);

            var weekendDays = WeekendDays.Get(Context.ExecutionContext).Split('|').Select(t => (DayOfWeek)int.Parse(t)).ToList();
            var holidays = new List<DateTime>();

            var holidaysQuery = HolidaysQuery.Get(Context.ExecutionContext);

            if (!string.IsNullOrEmpty(holidaysQuery))
            {
                holidays = QueryWithPaging(new FetchExpression(holidaysQuery))
                        .Where(t => t.Contains("holiday"))
                        .Select(t => t.GetAttributeValue<DateTime>("holiday").Date)
                        .ToList();
            }

            while (daysToAdd > 0)
            {
                result = result.AddDays(1);

                if (weekendDays.Contains(result.DayOfWeek))
                    continue;

                if (holidays.Contains(result.Date))
                    continue;

                daysToAdd--;
            }

            Result.Set(Context.ExecutionContext, result);
        }
    }
}
