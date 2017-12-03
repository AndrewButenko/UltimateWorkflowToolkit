using System.Activities;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.MultiselectHelper
{
    public class FieldContainsData: MultiselectWorkflowBase
    {

        #region Input/Output Parameters

        [Output("Field Contains Value")]
        public OutArgument<bool> IsFieldContainsData { get; set; }

        #endregion

        protected override void ExecuteWorkflowLogic()
        {
            IsFieldContainsData.Set(Context.ExecutionContext, Record.Contains(FieldName));
        }
    }
}
