using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateWorkflowToolkit.MultiselectHelper
{
    public class FieldContainsOneOfValues : FieldOperationBase
    {
        #region Input/Output Parameters

        [Output("Field Contains One of Values")]
        public OutArgument<bool> IsFieldContainOneofValues { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic()
        {
            if (string.IsNullOrEmpty(Values))
                throw new InvalidPluginExecutionException("Values parameter is empty");

            List<int> intValues = ConvertStringToIntArray(Values);

            if (!Record.Contains(FieldName))
            {
                IsFieldContainOneofValues.Set(Context.ExecutionContext, false);
                return;
            }

            var currentFieldValue = Record.GetAttributeValue<OptionSetValueCollection>(FieldName);

            var result = false;

            foreach (var picklistValue in currentFieldValue)
            {
                if (intValues.Contains(picklistValue.Value))
                {
                    result = true;
                    break;
                }
            }

            IsFieldContainOneofValues.Set(Context.ExecutionContext, result);
        }
    }
}
