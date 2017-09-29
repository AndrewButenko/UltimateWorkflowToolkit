using System.Activities;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.CoreOperations.Relationships
{
    public class CascadeStatus : RelationshipOperationBase
    {
        #region Input Parameters

        [Input("State Code for Child Record")]
        [RequiredArgument]
        public InArgument<int> StateCode { get; set; }

        [Input("Status Code for Child Record")]
        [RequiredArgument]
        public InArgument<int> StatusCode { get; set; }

        #endregion Input Parameters

        public override void PerformRelationshipOperation(Entity childRecord)
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
