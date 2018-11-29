using System;
using System.Xml;
using System.Linq;
using System.Activities;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using Newtonsoft.Json;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata.Query;

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

        protected abstract void ExecuteWorkflowLogic();

        protected override void Execute(CodeActivityContext executionContext)
        {
            Context = new WorkflowContext(executionContext);

            #region Log All Inputs

            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            properties.ToList().ForEach(p =>
            {
                if (p.PropertyType.IsSubclassOf(typeof(InArgument)) ||
                    p.PropertyType.IsSubclassOf(typeof(InOutArgument)))
                {

                    var propertyLabel = ((InputAttribute)p.GetCustomAttribute(typeof(InputAttribute))).Name;

                    var logText = $"Value of '{propertyLabel}' attribute equals to ";

                    var property = (Argument)p.GetValue(this);
                    var propertyValue = property.Get(executionContext);

                    if (propertyValue == null)
                        logText += "empty";
                    else if (propertyValue is string ||
                                propertyValue is decimal ||
                                propertyValue is int ||
                                propertyValue is bool)
                        logText += propertyValue.ToString();
                    else if (propertyValue is DateTime)
                        logText += ((DateTime)propertyValue).ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz");
                    else if (propertyValue is EntityReference)
                    {
                        var er = (EntityReference)propertyValue;
                        logText += $"Id: {er.Id}, LogicalName: {er.LogicalName}";
                    }
                    else if (propertyValue is OptionSetValue)
                        logText += ((OptionSetValue)propertyValue).Value;
                    else if (propertyValue is Money)
                        logText += ((Money)propertyValue).Value.ToString(CultureInfo.InvariantCulture);
                    else logText += $"undefined type - {p.GetType().FullName}";

                    Context.TracingService.Trace(logText);
                }
            });

            #endregion Log All Inputs

            try
            {
                ExecuteWorkflowLogic();

                IsExceptionOccured.Set(executionContext, false);
            }
            catch (Exception e)
            {
                if (IsThrowException.Get(Context.ExecutionContext) || Context.IsSyncMode)
                    throw;

                IsExceptionOccured.Set(executionContext, true);
                ErrorMessage.Set(executionContext, e.Message);
            }
        }

        public EntityReference ConvertToEntityReference(string recordReference)
        {
            Uri uriResult;

            if (Uri.TryCreate(recordReference, UriKind.Absolute, out uriResult))
            {
                return ParseUrlToEntityReference(recordReference);
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

        public List<Entity> QueryWithPaging(QueryBase query)
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
                initialFetchXml = ((FetchExpression)query).Query;
                var fetchXml = CreateFetchXml(initialFetchXml, null, 1, 500);
                ((FetchExpression)query).Query = fetchXml;
            }
            else
                throw new Exception($"Paging for {query.GetType().FullName} is not supported yet!");

            EntityCollection records;

            do
            {
                records = Context.UserService.RetrieveMultiple(query);

                results.AddRange(records.Entities);

                if (query is QueryByAttribute)
                {
                    ((QueryByAttribute)query).PageInfo.PageNumber++;
                    ((QueryByAttribute)query).PageInfo.PagingCookie = records.PagingCookie;
                }
                else if (query is QueryExpression)
                {
                    ((QueryExpression)query).PageInfo.PageNumber++;
                    ((QueryExpression)query).PageInfo.PagingCookie = records.PagingCookie;
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

        public WorkflowContext Context
        {
            get;
            private set;
        }

        public string SerializeDictionary(Dictionary<string, object> dictionary)
        {
            var rootElement = new XElement("Request");

            foreach (var key in dictionary.Keys)
            {
                var childElement = new XElement(key);

                var value = dictionary[key];

                if (value == null)
                {
                    var isnullAttribute = new XAttribute("IsNull", true);
                    childElement.Add(isnullAttribute);
                }
                else
                {
                    var typeName = value.GetType().ToString();

                    var typeAttribute = new XAttribute("Type", typeName);
                    childElement.Add(typeAttribute);

                    switch (typeName)
                    {
                        case "System.Boolean":
                        case "System.String":
                        case "System.Int32":
                        case "System.DateTime":
                        case "System.Decimal":
                            childElement.SetValue(value);
                            break;
                        case "Microsoft.Xrm.Sdk.OptionSetValue":
                            childElement.SetValue(((OptionSetValue)value).Value);
                            break;
                        case "Microsoft.Xrm.Sdk.Money":
                            childElement.SetValue(((Money)value).Value);
                            break;
                        case "Microsoft.Xrm.Sdk.EntityReference":
                            var entityReference = (EntityReference)value;
                            childElement.Add(new XElement("Id", entityReference.Id));
                            childElement.Add(new XElement("LogicalName", entityReference.LogicalName));
                            break;
                        default:
                            throw new InvalidPluginExecutionException($"Serialization is not implemented for {typeName} class");
                    }
                }

                rootElement.Add(childElement);
            }

            return rootElement.ToString();
        }

        public Dictionary<string, object> DeserializeDictionary(string dictionaryString)
        {
            var result = new Dictionary<string, object>();

            if (string.IsNullOrEmpty(dictionaryString))
                return result;

            var request = XElement.Parse(dictionaryString);

            request.Elements().ToList().ForEach(e =>
            {
                object fieldValue;

                if (e.Attribute("IsNull")?.Value == "true")
                    fieldValue = null;
                else
                {
                    if (e.Attribute("Type") == null)
                        throw new InvalidPluginExecutionException(
                            $"Attribute {e.Name} is not null and doesn't contain field type, can't deserialize");

                    var typeName = e.Attribute("Type").Value;

                    switch (typeName)
                    {
                        case "System.Boolean":
                            fieldValue = bool.Parse(e.Value);
                            break;
                        case "System.String":
                            fieldValue = e.Value;
                            break;
                        case "System.Int32":
                            fieldValue = int.Parse(e.Value);
                            break;
                        case "System.DateTime":
                            fieldValue = DateTime.Parse(e.Value);
                            break;
                        case "System.Decimal":
                            fieldValue = decimal.Parse(e.Value);
                            break;
                        case "Microsoft.Xrm.Sdk.OptionSetValue":
                            fieldValue = new OptionSetValue(int.Parse(e.Value));
                            break;
                        case "Microsoft.Xrm.Sdk.Money":
                            fieldValue = new Money(decimal.Parse(e.Value));
                            break;
                        case "Microsoft.Xrm.Sdk.EntityReference":
                            if (e.Element("Id") == null)
                                throw new InvalidPluginExecutionException(
                                    $"Can't parse {e.Name} node with {typeName} type - Id node is not available");

                            if (e.Element("LogicalName") == null)
                                throw new InvalidPluginExecutionException(
                                    $"Can't parse {e.Name} node with {typeName} type - LogicalName node is not available");

                            fieldValue = new EntityReference(e.Element("LogicalName").Value,
                                new Guid(e.Element("Id").Value));
                            break;
                        default:
                            throw new InvalidPluginExecutionException(
                                $"Serialization is not implemented for {typeName} class");
                    }

                    result.Add(e.Name.ToString(), fieldValue);
                }
            });

            return result;
        }

        private EntityReference ParseUrlToEntityReference(string url)
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

            var response = (RetrieveMetadataChangesResponse)Context.SystemService.Execute(retrieveMetadataChangesRequest);

            if (response.EntityMetadata.Count >= 1)
            {
                return new EntityReference(response.EntityMetadata[0].LogicalName, id);
            }

            return null;
        }

        private string CreateFetchXml(string initialFetchXml, string pagingCookie, int pageNumber, int fetchCount)
        {
            var doc = new XmlDocument();
            doc.LoadXml(initialFetchXml);

            if (doc.DocumentElement == null)
                throw new InvalidPluginExecutionException("Document element of Xml is empty!");

            var attrs = doc.DocumentElement.Attributes;

            if (!string.IsNullOrEmpty(pagingCookie))
            {
                var pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = pagingCookie;
                attrs.Append(pagingAttr);
            }

            var pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = Convert.ToString(pageNumber);
            attrs.Append(pageAttr);

            var countAttr = doc.CreateAttribute("count");
            countAttr.Value = Convert.ToString(fetchCount);
            attrs.Append(countAttr);

            return doc.OuterXml;
        }
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
