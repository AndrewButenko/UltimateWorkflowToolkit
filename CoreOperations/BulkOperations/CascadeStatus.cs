using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using System.Collections.Generic;

namespace UltimateWorkflowToolkit.CoreOperations.BulkOperations
{
    public class CascadeStatus : BulkOperationBase
    {
        #region Input Parameters

        [Input("State Code for Child Record")]
        [RequiredArgument]
        public InArgument<int> StateCode { get; set; }

        [Input("Status Code for Child Record")]
        [RequiredArgument]
        public InArgument<int> StatusCode { get; set; }

        #endregion Input Parameters

        protected override void PerformOperation(List<Entity> childRecords, bool isContinueOnError)
        {
            foreach (var childRecord in childRecords)
            {
                try
                {
                    Context.UserService.Execute(new SetStateRequest()
                    {
                        EntityMoniker = childRecord.ToEntityReference(),
                        State = new OptionSetValue(StateCode.Get(Context.ExecutionContext)),
                        Status = new OptionSetValue(StatusCode.Get(Context.ExecutionContext))
                    });
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
