using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
