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

        protected override void ExecuteWorkflowLogic()
        {
            var attachmentsQuery = new QueryByAttribute("activitymimeattachment")
            {
                ColumnSet = new ColumnSet("body", "filename")
            };
            attachmentsQuery.AddAttributeValue("objectid", Email.Get(Context.ExecutionContext).Id);

            var attachments = QueryWithPaging(attachmentsQuery);

            var record = ConvertToEntityReference(Record.Get(Context.ExecutionContext));

            attachments.ForEach(attachment =>
            {
                var note = new Entity("annotation")
                {
                    ["documentbody"] = attachment["body"],
                    ["filename"] = attachment["filename"],
                    ["isdocument"] = true,
                    ["objectid"] = record
                };

                Context.UserService.Create(note);
            });
        }

        #endregion Overriddes

    }
}
