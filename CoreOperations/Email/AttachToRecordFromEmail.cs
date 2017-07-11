using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Email
{
    public class AttachToRecordFromEmail : CrmWorkflowBase
    {

        #region Inputs

        [Input("Email")]
        [RequiredArgument]
        [ReferenceTarget("email")]
        public InArgument<EntityReference> Email { get; set; }

        [Input("Record Reference")]
        [RequiredArgument]
        public InArgument<string> Record { get; set; }


        #endregion Inputs

        #region Overriddes

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service,
            IOrganizationService sysService)
        {
            var attachmentsQuery = new QueryByAttribute("activitymimeattachment")
            {
                ColumnSet = new ColumnSet("body", "filename")
            };
            attachmentsQuery.AddAttributeValue("objectid", Email.Get(executionContext).Id);

            var attachments = QueryWithPaging(attachmentsQuery, sysService);

            var record = ConvertToEntityReference(Record.Get(executionContext), sysService);

            attachments.ForEach(attachment =>
            {
                var note = new Entity("annotation")
                {
                    ["documentbody"] = attachment["body"],
                    ["filename"] = attachment["filename"],
                    ["isdocument"] = true,
                    ["objectid"] = record
                };

                service.Create(note);
            });
        }

        #endregion Overriddes

    }
}
