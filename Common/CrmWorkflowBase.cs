using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

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
            IOrganizationService service);

        #endregion Abstracts methods    

        #region Overrides

        protected override void Execute(CodeActivityContext executionContext)
        {
            var context = executionContext.GetExtension<IWorkflowContext>();
            var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            var service = serviceFactory.CreateOrganizationService(context.UserId);

            //ToDo: Include validation of InArguments that are marked as required

            try
            {
                ExecuteWorkflowLogic(executionContext, context, service);

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
