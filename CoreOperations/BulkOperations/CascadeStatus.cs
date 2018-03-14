using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;

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

        protected override void PerformOperation(Entity childRecord)
        {
            Context.UserService.Execute(new SetStateRequest()
            {
                EntityMoniker = childRecord.ToEntityReference(),
                State = new OptionSetValue(StateCode.Get(Context.ExecutionContext)),
                Status = new OptionSetValue(StatusCode.Get(Context.ExecutionContext))
            });
        }
    }
}
