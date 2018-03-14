using System.Activities;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Common
{
    public class GetRecordId: CrmWorkflowBase
    {
        #region Input/Output Arguments

        [Input("Record Reference")]
        [RequiredArgument]
        public InArgument<string> Record { get; set; }

        [Output("Id")]
        public OutArgument<string> RecordId { get; set; }

        [Output("Entity Type Name")]
        public OutArgument<string> EntityTypeName { get; set; }

        #endregion Input/Output Arguments

        protected override void ExecuteWorkflowLogic()
        {
            var target = ConvertToEntityReference(Record.Get(Context.ExecutionContext));

            RecordId.Set(Context.ExecutionContext, target.Id.ToString());
            EntityTypeName.Set(Context.ExecutionContext, target.LogicalName);
        }
    }
}
