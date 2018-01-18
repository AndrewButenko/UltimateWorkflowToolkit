using System.Linq;
using Microsoft.Xrm.Sdk;

namespace UltimateWorkflowToolkit.MultiselectHelper
{
    public class SetFieldValue : FieldOperationBase
    {
        protected override void ExecuteWorkflowLogic()
        {
            if (string.IsNullOrEmpty(Values))
                throw new InvalidPluginExecutionException("Values parameter is empty");

            var intValues = ConvertStringToIntArray(Values).Select(o => new OptionSetValue(o)).ToList();

            var record = Record;

            record[FieldName] = new OptionSetValueCollection(intValues);

            Context.UserService.Update(record);
        }
    }
}
