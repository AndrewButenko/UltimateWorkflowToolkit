using System.Collections.Generic;
using UltimateWorkflowToolkit.CoreOperations.Base;

namespace UltimateWorkflowToolkit.CoreOperations.RequestBuilder
{
    public class SetFieldToNull: SetFieldWorkflowBase
    {
        protected override void AddField(ref Dictionary<string, object> dictionary, string fieldName)
        {
            dictionary.Add(fieldName, null);
        }
    }
}
