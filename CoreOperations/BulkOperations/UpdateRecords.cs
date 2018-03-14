using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.CoreOperations.BulkOperations
{
    public class UpdateRecords : BulkOperationBase
    {
        #region Inputs

        [Input("Request")]
        public InArgument<string> SerializedObject { get; set; }

        #endregion Inputs

        protected override void PerformOperation(Entity childRecord)
        {
            var request = DeserializeDictionary(SerializedObject.Get(Context.ExecutionContext));

            foreach (var key in request.Keys)
            {
                childRecord[key] = request[key];
            }

            Context.UserService.Update(childRecord);
        }
    }
}
