using System;
using System.Activities;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Relationships
{
    public abstract class RelationshipOperationBase : CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Record")]
        [RequiredArgument]
        public InArgument<string> Record { get; set; }

        [Input("Relationship Name")]
        [RequiredArgument]
        public InArgument<string> RelationshipName { get; set; }

        [Input("Additional Filter Xml")]
        public InArgument<string> AdditionalFilterArgument { get; set; }

        #endregion Input/Output Parameters

        #region Abstract Methods

        protected abstract void PerformRelationshipOperation(Entity childRecord);

        #endregion Abstract Methods

        protected override void ExecuteWorkflowLogic()
        {
            var record = ConvertToEntityReference(Record.Get(Context.ExecutionContext));
            var relationshipName = RelationshipName.Get(Context.ExecutionContext);
            var relationship = new Relationship(relationshipName);

            var retrieveEntityRequest = new RetrieveEntityRequest()
            {
                LogicalName = record.LogicalName,
                EntityFilters = EntityFilters.Relationships,
                RetrieveAsIfPublished = true
            };

            var retrieveEntityResponse = (RetrieveEntityResponse) Context.SystemService.Execute(retrieveEntityRequest);

            string childEntityName = null;

            var ntonRelationship =
                retrieveEntityResponse.EntityMetadata.ManyToManyRelationships.FirstOrDefault(
                    r => r.SchemaName == relationshipName);

            if (ntonRelationship != null)
            {
                childEntityName = ntonRelationship.Entity1LogicalName == record.LogicalName
                    ? ntonRelationship.Entity2LogicalName
                    : ntonRelationship.Entity1LogicalName;
            }
            else
            {
                var onetonRelationship =
                    retrieveEntityResponse.EntityMetadata.OneToManyRelationships.FirstOrDefault(
                        r => r.SchemaName == relationshipName);

                if (onetonRelationship == null)
                    throw new Exception($"Relationship {relationshipName} is not available for {record.LogicalName} entity");

                childEntityName = onetonRelationship.ReferencingEntity;
            }

            var childRecordsFetchXml = $@"
                <fetch>
                  <entity name='{childEntityName}'>
                    <attribute name='{childEntityName}id' />";

            var additionalConditions = AdditionalFilterArgument.Get(Context.ExecutionContext);

            if (!string.IsNullOrEmpty(additionalConditions))
                childRecordsFetchXml += $"<filter type='and'>{additionalConditions.Replace('"', '\'')}</filter>";

            childRecordsFetchXml += @"
                    </entity>
                </fetch>";

            var retrieveRequest = new RetrieveRequest()
            {
                ColumnSet = new ColumnSet(false),
                Target = record,
                RelatedEntitiesQuery = new RelationshipQueryCollection
                {
                    {relationship, new FetchExpression(childRecordsFetchXml)}
                }
            };

            var retrieveResponse = (RetrieveResponse) Context.UserService.Execute(retrieveRequest);

            var childRecords = retrieveResponse.Entity.RelatedEntities[relationship].Entities.ToList();

            foreach (var childRecord in childRecords)
            {
                PerformRelationshipOperation(childRecord);
            }
        }
    }
}
