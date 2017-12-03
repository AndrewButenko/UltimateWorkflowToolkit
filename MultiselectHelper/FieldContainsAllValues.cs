using System.Linq;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.MultiselectHelper
{
    public class FieldContainsAllValues : FieldOperationBase
    {
        #region Input/Output Parameters

        [Output("Field Contains One of Values")]
        public OutArgument<bool> IsFieldContainAllValues { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic()
        {
            if (!Record.Contains(FieldName))
            {
                IsFieldContainAllValues.Set(Context.ExecutionContext, false);
                return;
            }

            if (string.IsNullOrEmpty(Values))
                throw new InvalidPluginExecutionException("Values parameter is empty");

            var intValues = ConvertStringToIntArray(Values);

            var currentFieldValue = Record.GetAttributeValue<OptionSetValueCollection>(FieldName).Select(o => o.Value).ToList();

            var result = true;

            foreach (var intValue in intValues)
            {
                if (!currentFieldValue.Contains(intValue))
                {
                    result = false;
                    break;
                }
            }

            IsFieldContainAllValues.Set(Context.ExecutionContext, result);
        }
    }
}
