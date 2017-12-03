using System.Linq;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace UltimateWorkflowToolkit.MultiselectHelper
{
    public class RemoveValuesFromField : FieldOperationBase
    {
        protected override void ExecuteWorkflowLogic()
        {
            if (string.IsNullOrEmpty(Values))
                throw new InvalidPluginExecutionException("Values parameter is empty");

            var intValues = ConvertStringToIntArray(Values);

            var currentFieldValue = Record.GetAttributeValue<OptionSetValueCollection>(FieldName)?.ToList();

            if (currentFieldValue == null)
                currentFieldValue = new List<OptionSetValue>();

            foreach (var intValue in intValues)
            {
                var existingOption = currentFieldValue.FirstOrDefault(o => o.Value == intValue);
                if (existingOption == null)
                    continue;

                currentFieldValue.Remove(existingOption);
            }

            var record = Record;
            record[FieldName] = new OptionSetValueCollection(currentFieldValue);

            Context.UserService.Update(record);
        }
    }
}
