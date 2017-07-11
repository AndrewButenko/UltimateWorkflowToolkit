using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.CoreOperations.Email
{
    public class AttachNotesToEmailAndSend : SendEmail
    {
        #region Inputs/Outputs

        [Input("Record Reference")]
        [RequiredArgument]
        public InArgument<string> Record { get; set; }

        [Input("Send Email")]
        [RequiredArgument]
        [Default("True")]
        public InArgument<bool> SendAfterOperation { get; set; }

        #endregion Inputs/Outputs

        #region Overriddes 

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service,
            IOrganizationService sysService)
        {
            var sourceRecord = ConvertToEntityReference(Record.Get(executionContext), sysService);

            var notesQuery = new QueryByAttribute("annotation")
            {
                ColumnSet = new ColumnSet("documentbody", "filename")
            };
            notesQuery.AddAttributeValue("isdocument", true);
            notesQuery.AddAttributeValue("objectid", sourceRecord.Id);

            var notes = QueryWithPaging(notesQuery, service);

            notes.ForEach(note =>
            {
                var activityattachment = new Entity("activitymimeattachment")
                {
                    ["objectid"] = Email.Get(executionContext),
                    ["objecttypecode"] = "email",
                    ["body"] = note["documentbody"],
                    ["filename"] = note["filename"]
                };

                service.Create(activityattachment);
            });

            if (SendAfterOperation.Get(executionContext))
                base.ExecuteWorkflowLogic(executionContext, context, service, sysService);
        }

        #endregion Overriddes 
    }
}
