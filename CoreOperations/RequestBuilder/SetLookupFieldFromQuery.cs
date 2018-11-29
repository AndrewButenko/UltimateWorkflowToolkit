using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;
using UltimateWorkflowToolkit.CoreOperations.Base;

namespace UltimateWorkflowToolkit.CoreOperations.RequestBuilder
{
    public class SetLookupFieldFromQuery: SetFieldWorkflowBase
    {
        #region Inputs

        [Input("Fetch Xml Query")]
        [RequiredArgument]
        public InArgument<string> FetchXmlQuery { get; set; }

        [Input("Set to null on empty results")]
        [RequiredArgument]
        public InArgument<bool> IsSetToNull { get; set; }

        #endregion Inputs

        protected override void AddField(ref Dictionary<string, object> request, string fieldName)
        {
            var fetchXml = FetchXmlQuery.Get(Context.ExecutionContext);

            var record = Context.UserService.RetrieveMultiple(new FetchExpression(fetchXml)).Entities
                .FirstOrDefault();

            if (record != null)
            {
                request.Add(fieldName, record.ToEntityReference());
            }
            else if (IsSetToNull.Get(Context.ExecutionContext))
            {
                request.Add(fieldName, null);
            }
        }
    }
}
