using System;
using System.Xml;
using System.Linq;
using System.Activities;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.Common
{
    public abstract class CrmWorkflowBase : CodeActivity
    {
        #region Input/Output Parameters

        [Input("Throw an Exception on Error")]
        [RequiredArgument]
        [Default("True")]
        public InArgument<bool> IsThrowException { get; set; }

        [Output("Error Occured")]
        public OutArgument<bool> IsExceptionOccured { get; set; }

        [Output("Error Message")]
        public OutArgument<string> ErrorMessage { get; set; }

        #endregion

        #region Abstract methods

        protected abstract void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context,
            IOrganizationService service, IOrganizationService sysService);

        #endregion Abstracts methods    

        #region Overrides

        protected override void Execute(CodeActivityContext executionContext)
        {
            var context = executionContext.GetExtension<IWorkflowContext>();
            var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            var service = serviceFactory.CreateOrganizationService(context.UserId);
            var systemService = serviceFactory.CreateOrganizationService(null);
            var tracingService = executionContext.GetExtension<ITracingService>();

            #region Log All Inputs

            var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            properties.ToList().ForEach(p =>
            {
                tracingService.Trace(p.PropertyType.FullName);
            });

            #endregion Log All Inputs

            //ToDo: Include validation of InArguments that are marked as required

            try
            {
                ExecuteWorkflowLogic(executionContext, context, service, systemService);

                IsExceptionOccured.Set(executionContext, false);
            }
            catch (Exception e)
            {
                if (IsThrowException.Get(executionContext) || context.WorkflowMode == (int)WorkflowExecutionMode.RealTime)
                    throw;

                IsExceptionOccured.Set(executionContext, true);
                ErrorMessage.Set(executionContext, e.Message);
            }
        }

        #endregion Overrides

        #region Publics

        public EntityReference ConvertToEntityReference(string recordReference, IOrganizationService service)
        {
            Uri uriResult;

            if (Uri.TryCreate(recordReference, UriKind.Absolute, out uriResult))
            {
                return ParseUrlToEntityReference(recordReference, service);
            }

            try
            {
                var jsonEntityReference = JsonConvert.DeserializeObject<JsonEntityReference>(recordReference);

                return new EntityReference(jsonEntityReference.LogicName, jsonEntityReference.Id);
            }
            catch (Exception e)
            {
                throw new Exception($"Error converting string '{recordReference}' to EntityReference - {e.Message}", e);
            }
        }

        public List<Entity> QueryWithPaging(QueryBase query, IOrganizationService service)
        {
            var results = new List<Entity>();
            var initialFetchXml = string.Empty;
            var pageNumber = 1;

            if (query is QueryByAttribute)
                ((QueryByAttribute)query).PageInfo = new PagingInfo()
                {
                    Count = 500,
                    PageNumber = 1
                };
            else if (query is QueryExpression)
                ((QueryExpression)query).PageInfo = new PagingInfo()
                {
                    Count = 500,
                    PageNumber = 1
                };
            else if (query is FetchExpression)
            {
                initialFetchXml = ((FetchExpression) query).Query;
                var fetchXml = CreateFetchXml(initialFetchXml, null, 1, 500);
                ((FetchExpression) query).Query = fetchXml;
            }
            else
                throw new Exception($"Paging for {query.GetType().FullName} is not supported yet!");

            EntityCollection records;

            do
            {
                records = service.RetrieveMultiple(query);

                results.AddRange(records.Entities);

                if (query is QueryByAttribute)
                {
                    ((QueryByAttribute)query).PageInfo.PageNumber++;
                    ((QueryByAttribute)query).PageInfo.PagingCookie = records.PagingCookie;
                }
                else if (query is QueryExpression)
                {
                    ((QueryExpression) query).PageInfo.PageNumber++;
                    ((QueryExpression) query).PageInfo.PagingCookie = records.PagingCookie;
                }
                else
                {
                    pageNumber++;
                    var fetchXml = CreateFetchXml(initialFetchXml, records.PagingCookie, pageNumber, 500);
                    ((FetchExpression)query).Query = fetchXml;
                }
            } while (records.MoreRecords);

            return results;
        }

        #endregion Publics

        #region Privates

        private EntityReference ParseUrlToEntityReference(string url, IOrganizationService service)
        {
            var uri = new Uri(url);

            var found = 0;
            int entityTypeCode = 0;
            var id = Guid.Empty;

            var parameters = uri.Query.TrimStart('?').Split('&');
            foreach (var param in parameters)
            {
                var nameValue = param.Split('=');
                switch (nameValue[0])
                {
                    case "etc":
                        entityTypeCode = int.Parse(nameValue[1]);
                        found++;
                        break;
                    case "id":
                        id = new Guid(nameValue[1]);
                        found++;
                        break;
                }
                if (found > 1) break;
            }

            if (id == Guid.Empty)
                return null;

            var entityFilter = new MetadataFilterExpression(LogicalOperator.And);
            entityFilter.Conditions.Add(new MetadataConditionExpression("ObjectTypeCode ", MetadataConditionOperator.Equals, entityTypeCode));
            var propertyExpression = new MetadataPropertiesExpression { AllProperties = false };
            propertyExpression.PropertyNames.Add("LogicalName");
            var entityQueryExpression = new EntityQueryExpression()
            {
                Criteria = entityFilter,
                Properties = propertyExpression
            };

            var retrieveMetadataChangesRequest = new RetrieveMetadataChangesRequest()
            {
                Query = entityQueryExpression
            };

            var response = (RetrieveMetadataChangesResponse)service.Execute(retrieveMetadataChangesRequest);

            if (response.EntityMetadata.Count >= 1)
            {
                return  new EntityReference(response.EntityMetadata[0].LogicalName, id);
            }

            return null;
        }

        private string CreateFetchXml(string initialFetchXml, string pagingCookie, int pageNumber, int fetchCount)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(initialFetchXml);

            var attrs = doc.DocumentElement.Attributes;

            if (!string.IsNullOrEmpty(pagingCookie))
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = pagingCookie;
                attrs.Append(pagingAttr);
            }

            var pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = Convert.ToString(pageNumber);
            attrs.Append(pageAttr);

            var countAttr = doc.CreateAttribute("count");
            countAttr.Value = System.Convert.ToString(fetchCount);
            attrs.Append(countAttr);

            return doc.OuterXml;
        }

        #endregion Privates

    }

    internal enum WorkflowExecutionMode : int
    {
        Asynchronous = 0,
        RealTime = 1
    }

    public class JsonEntityReference
    {
        [JsonProperty(PropertyName = "entityType")]
        public string LogicName { get; set; }

        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
    }
}
