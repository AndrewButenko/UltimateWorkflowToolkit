using System;
using System.Activities;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Security
{
    public abstract class ShareSecuredFieldsBase : CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Record Reference")]
        [RequiredArgument]
        public InArgument<string> Record { get; set; }

        [Input("Fields List (Comma Separated)")]
        [RequiredArgument]
        public InArgument<string> Fields { get; set; }

        [Input("Read Access")]
        [RequiredArgument]
        [Default("False")]
        public InArgument<bool> ReadAccess { get; set; }

        [Input("Write Access")]
        [RequiredArgument]
        [Default("False")]
        public InArgument<bool> WriteAccess { get; set; }

        #endregion Input/Output Parameters

        #region Abstract Methods

        public abstract EntityReference Principal { get; }

        #endregion Abstract Methods

        protected override void ExecuteWorkflowLogic()
        {
            var target = ConvertToEntityReference(Record.Get(Context.ExecutionContext));
            var principal = Principal;
            var readAccess = ReadAccess.Get(Context.ExecutionContext);
            var writeAccess = WriteAccess.Get(Context.ExecutionContext);

            //Let's retrieve attributes first
            var retrieveEntityRequest = new RetrieveEntityRequest()
            {
                LogicalName = target.LogicalName,
                EntityFilters = EntityFilters.Attributes,
                RetrieveAsIfPublished = true
            };

            var retrieveEntityResponse = (RetrieveEntityResponse) Context.SystemService.Execute(retrieveEntityRequest);

            var fields = Fields.Get(Context.ExecutionContext).ToLowerInvariant().Split(',').ToArray();

            foreach (var field in fields)
            {
                var crmAttribute =
                    retrieveEntityResponse.EntityMetadata.Attributes.FirstOrDefault(a => a.LogicalName == field);

                if (crmAttribute == null)
                    throw new Exception($"{field} attribute is not available in {target.LogicalName} entity");

                if (!crmAttribute.IsSecured.Value)
                    throw new Exception($"{field} attribute is not secured");

                var queryPOAA = new QueryByAttribute("principalobjectattributeaccess")
                {
                    ColumnSet = new ColumnSet("readaccess", "updateaccess")
                };
                queryPOAA.AddAttributeValue("attributeid", crmAttribute.MetadataId.Value);
                queryPOAA.AddAttributeValue("objectid", target.Id);
                queryPOAA.AddAttributeValue("principalid", principal.Id);

                var poaa = Context.SystemService.RetrieveMultiple(queryPOAA).Entities.FirstOrDefault();

                if (poaa != null)
                {
                    if (readAccess || writeAccess)
                    {
                        poaa["readaccess"] = readAccess;
                        poaa["updateaccess"] = writeAccess;
                        Context.SystemService.Update(poaa);
                    }
                    else
                        Context.SystemService.Delete("principalobjectattributeaccess", poaa.Id);
                }
                else if (readAccess || writeAccess)
                {
                    poaa = new Entity("principalobjectattributeaccess")
                    {
                        ["attributeid"] = crmAttribute.MetadataId.Value,
                        ["objectid"] = target,
                        ["readaccess"] = readAccess,
                        ["updateaccess"] = writeAccess,
                        ["principalid"] = principal
                    };

                    Context.SystemService.Create(poaa);
                }
            }
        }
    }
}