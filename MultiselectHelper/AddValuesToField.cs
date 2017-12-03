using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateWorkflowToolkit.MultiselectHelper
{
    public class AddValuesToField : FieldOperationBase
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
                if (existingOption != null)
                    continue;

                currentFieldValue.Add(new OptionSetValue(intValue));
            }

            var record = Record;
            record[FieldName] = new OptionSetValueCollection(currentFieldValue);

            Context.UserService.Update(record);
        }
    }
}
