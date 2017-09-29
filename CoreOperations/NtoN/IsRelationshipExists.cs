using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.NtoN
{
    public class IsRelationshipExists : CrmWorkflowBase
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

        [Output("Is Relationship Exists")]
        public OutArgument<bool> RelationshipExists { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic()
        {
            var record1 = ConvertToEntityReference(Record1Id.Get(Context.ExecutionContext));
            var record2 = ConvertToEntityReference(Record2Id.Get(Context.ExecutionContext));

            var record2Query = new QueryByAttribute(record2.LogicalName)
            {
                ColumnSet = new ColumnSet(false)
            };
            record2Query.AddAttributeValue($"{record2.LogicalName}id", record2.Id);

            var relationship = new Relationship(RelationshipName.Get(Context.ExecutionContext));

            var retrieveRequest = new RetrieveRequest()
            {
                ColumnSet = new ColumnSet(false),
                Target = record1,
                RelatedEntitiesQuery = new RelationshipQueryCollection
                {
                    {relationship, record2Query}
                }
            };

            var retrieveResponse = (RetrieveResponse) Context.SystemService.Execute(retrieveRequest);

            var isRelationshipExists = retrieveResponse.Entity.RelatedEntities[relationship].Entities.Count != 0;

            RelationshipExists.Set(Context.ExecutionContext, isRelationshipExists);
        }
    }
}
