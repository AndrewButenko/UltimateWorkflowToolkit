using System;
using System.Linq;
using System.Activities;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.BulkOperations
{
    public abstract class BulkOperationBase : CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Record")]
        public InArgument<string> Record { get; set; }

        [Input("Relationship Name")]
        public InArgument<string> RelationshipName { get; set; }

        [Input("Additional Filter Xml")]
        public InArgument<string> AdditionalFilterArgument { get; set; }

        [Input("Public View")]
        [ReferenceTarget("savedquery")]
        public InArgument<EntityReference> PublicView { get; set; }

        [Input("Private View")]
        [ReferenceTarget("userquery")]
        public InArgument<EntityReference> PrivateView { get; set; }

        [Input("FetchXml Query")]
        public InArgument<string> FetchXmlQuery { get; set; }

        [Input("Continue After First Error")]
        [RequiredArgument]
        public InArgument<bool> IsContinueOnError { get; set; }

        #endregion Input/Output Parameters

        protected abstract void PerformOperation(Entity childRecord);

        protected override void ExecuteWorkflowLogic()
        {
            List<Entity> records;

            var recordUrl = Record.Get(Context.ExecutionContext);

            if (!string.IsNullOrEmpty(recordUrl))
            {
                var record = ConvertToEntityReference(Record.Get(Context.ExecutionContext));
                var relationshipName = RelationshipName.Get(Context.ExecutionContext);

                if (string.IsNullOrEmpty(relationshipName))
                {
                    throw new InvalidPluginExecutionException("Reationship Name Parameter is empty!");
                }

                var relationship = new Relationship(relationshipName);

                var retrieveEntityRequest = new RetrieveEntityRequest()
                {
                    LogicalName = record.LogicalName,
                    EntityFilters = EntityFilters.Relationships,
                    RetrieveAsIfPublished = true
                };

                var retrieveEntityResponse =
                    (RetrieveEntityResponse) Context.SystemService.Execute(retrieveEntityRequest);

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
                        throw new Exception(
                            $"Relationship {relationshipName} is not available for {record.LogicalName} entity");

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

                records = retrieveResponse.Entity.RelatedEntities[relationship].Entities.ToList();
            }
            else
            {
                var publicView = PublicView.Get(Context.ExecutionContext);
                var privateView = PrivateView.Get(Context.ExecutionContext);
                var fetchXml = FetchXmlQuery.Get(Context.ExecutionContext);

                if (publicView == null &&
                    privateView == null &&
                    fetchXml == null)
                    throw new InvalidPluginExecutionException("One of 'Record Reference'/'Relationship Name', 'Public View', 'Private View' or 'Fetch Xml Query' inputs has to be populated!");

                if (publicView != null)
                {
                    fetchXml = Context.SystemService.Retrieve(publicView.LogicalName, publicView.Id, new ColumnSet("fetchxml")).GetAttributeValue<string>("fetchxml");
                }
                else if (privateView != null)
                {
                    fetchXml = Context.SystemService.Retrieve(privateView.LogicalName, privateView.Id, new ColumnSet("fetchxml")).GetAttributeValue<string>("fetchxml");
                }

                records = QueryWithPaging(new FetchExpression(fetchXml));
            }

            var isContinueOnError = IsContinueOnError.Get(Context.ExecutionContext);

            foreach (var childRecord in records)
            {
                try
                {
                    var recordForOperation = new Entity(childRecord.LogicalName, childRecord.Id);

                    PerformOperation(recordForOperation);
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
