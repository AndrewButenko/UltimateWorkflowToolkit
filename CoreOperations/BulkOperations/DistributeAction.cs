using System.Activities;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.CoreOperations.BulkOperations
{
    public class DistributeAction : BulkOperationBase
    {
        #region Inputs

        [Input("Request")]
        public InArgument<string> SerializedObject { get; set; }

        [Input("Action Name")]
        [RequiredArgument]
        public InArgument<string> ActionName { get; set; }

        #endregion Inputs
    
        protected override void PerformOperation(List<Entity> childRecords, bool isContinueOnError)
        {
            var request = DeserializeDictionary(SerializedObject.Get(Context.ExecutionContext));

            var organizationRequest = new OrganizationRequest(ActionName.Get(Context.ExecutionContext));

            foreach (var key in request.Keys)
            {
                organizationRequest[key] = request[key];
            }

            foreach (var childRecord in childRecords)
            {
                try
                {
                    organizationRequest["Target"] = childRecord.ToEntityReference();

                    Context.UserService.Execute(organizationRequest);
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
