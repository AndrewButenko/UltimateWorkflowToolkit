using System.Activities;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Email
{
    public class CreateEmailFromTemplate : CrmWorkflowBase
    {

        #region Inputs/Outputs

        [Input("Template")]
        [RequiredArgument]
        [ReferenceTarget("template")]
        public InArgument<EntityReference> Template { get; set; }

        [Input("Record Reference")]
        public InArgument<string> Record { get; set; }

        [Output("Email")]
        [ReferenceTarget("email")]
        public OutArgument<EntityReference> Email { get; set; }

        #endregion Inputs/Outputs

        #region Overriddes

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service,
            IOrganizationService sysService)
        {
            var templateId = Template.Get(executionContext).Id;

            var template = sysService.Retrieve("template", templateId, new ColumnSet("templatetypecode"));

            var templateTypeCode = template.GetAttributeValue<string>("templatetypecode");

            EntityReference targetRecord;

            if (templateTypeCode == "systemuser" && string.IsNullOrEmpty(Record.Get(executionContext)))
            {
                targetRecord = new EntityReference("systemuser", context.UserId);
            }
            else
            {
                targetRecord = ConvertToEntityReference(Record.Get(executionContext), sysService);
            }

            if (templateTypeCode != targetRecord.LogicalName)
                throw new InvalidPluginExecutionException("Entities from template and passed as parameter do not fit!");

            var instantiateTemplateResponse =
                (InstantiateTemplateResponse) service.Execute(new InstantiateTemplateRequest()
                {
                    ObjectId = targetRecord.Id,
                    ObjectType = targetRecord.LogicalName,
                    TemplateId = templateId
                });

            var email = instantiateTemplateResponse.EntityCollection[0];
            email["regardingobjectid"] = targetRecord;
            var emailId = service.Create(email);

            Email.Set(executionContext, new EntityReference("email", emailId));
        }

        #endregion Overriddes

    }
}
