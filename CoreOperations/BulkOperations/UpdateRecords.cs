using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Collections.Generic;

namespace UltimateWorkflowToolkit.CoreOperations.BulkOperations
{
    public class UpdateRecords : BulkOperationBase
    {
        #region Inputs

        [Input("Request")]
        public InArgument<string> SerializedObject { get; set; }

        #endregion Inputs

        protected override void PerformOperation(List<Entity> childRecords, bool isContinueOnError)
        {
            var request = DeserializeDictionary(SerializedObject.Get(Context.ExecutionContext));

            foreach (var childRecord in childRecords)
            {
                var recordToUpdate = new Entity(childRecord.LogicalName, childRecord.Id);

                foreach (var key in request.Keys)
                {
                    recordToUpdate[key] = request[key];
                }

                try
                {
                    Context.UserService.Update(recordToUpdate);
                }
                catch
                {
                    if (!isContinueOnError || Context.IsSyncMode)
                        throw;
                }
            }
        }
    }
}
