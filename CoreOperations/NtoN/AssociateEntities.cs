using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.NtoN
{
    public class AssociateEntities : CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Record 1 Reference")]
        [RequiredArgument]
        public InArgument<string> Record1Id { get; set; }

        [Input("Record 2 Reference")]
        [RequiredArgument]
        public InArgument<string> Record2Id { get; set; }

        [Input("Relationship Name")]
        [RequiredArgument]
        public InArgument<string> RelationshipName { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic()
        {
            var record1 = ConvertToEntityReference(Record1Id.Get(Context.ExecutionContext));
            var record2 = ConvertToEntityReference(Record2Id.Get(Context.ExecutionContext));

            Context.UserService.Execute(new AssociateEntitiesRequest()
            {
                Moniker1 = record1,
                Moniker2 = record2,
                RelationshipName = RelationshipName.Get(Context.ExecutionContext)
            });
        }
    }
}
