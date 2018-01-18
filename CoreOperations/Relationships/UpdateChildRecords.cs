using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.CoreOperations.Relationships
{
    public class UpdateChildRecords : RelationshipOperationBase
    {
        #region Inputs

        [Input("Request")]
        public InArgument<string> SerializedObject { get; set; }

        #endregion Inputs

        protected override void PerformRelationshipOperation(Entity childRecord)
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
