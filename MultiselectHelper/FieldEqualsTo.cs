using System.Linq;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.MultiselectHelper
{
    public class FieldEqualsTo : FieldOperationBase
    {

        #region Input/Output Parameters

        [Output("Field Equals to Value")]
        public OutArgument<bool> IsFieldEquals { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic()
        {
            if (string.IsNullOrEmpty(Values) && !Record.Contains(FieldName))
                IsFieldEquals.Set(Context.ExecutionContext, true);
            else if ((string.IsNullOrEmpty(Values) && Record.Contains(FieldName)) ||
                !string.IsNullOrEmpty(Values) && !Record.Contains(FieldName))
                IsFieldEquals.Set(Context.ExecutionContext, false);
            else
            {
                var intValues = ConvertStringToIntArray(Values);
                var crmValues = Record.GetAttributeValue<OptionSetValueCollection>(FieldName).Select(o => o.Value).ToList();

                foreach (var intValue in intValues)
                {
                    if (!crmValues.Contains(intValue))
                    {
                        IsFieldEquals.Set(Context.ExecutionContext, false);
                        return;
                    }
                }

                IsFieldEquals.Set(Context.ExecutionContext, true);
            }
        }
    }
}
