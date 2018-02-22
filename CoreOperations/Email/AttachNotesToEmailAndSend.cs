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

        [Input("Documents Filter")]
        public InArgument<string> FilterXml { get; set; }

        [Input("Send Email")]
        [RequiredArgument]
        [Default("True")]
        public InArgument<bool> SendAfterOperation { get; set; }

        #endregion Inputs/Outputs

        #region Overriddes 

        protected override void ExecuteWorkflowLogic()
        {
            var sourceRecord = ConvertToEntityReference(Record.Get(Context.ExecutionContext));

            var documentFilter = FilterXml.Get(Context.ExecutionContext);

            var documentsQuery = $@"
                <fetch>
                  <entity name='annotation'>
                    <attribute name='filename' />
                    <attribute name='documentbody' />
                    <filter type='and'>
                     <condition attribute='isdocument' operator='eq' value='1' />
                     <condition attribute='objectid' operator='eq' value='{sourceRecord.Id}' />";

            if (!string.IsNullOrEmpty(documentFilter))
                documentsQuery += documentFilter.Replace('"', '\'');

            documentsQuery += @"
                    </filter>
                  </entity>
                </fetch>";

            var notes = QueryWithPaging(new FetchExpression(documentsQuery));

            notes.ForEach(note =>
            {
                var activityattachment = new Entity("activitymimeattachment")
                {
                    ["objectid"] = Email.Get(Context.ExecutionContext),
                    ["objecttypecode"] = "email",
                    ["body"] = note["documentbody"],
                    ["filename"] = note["filename"]
                };

                Context.UserService.Create(activityattachment);
            });

            if (SendAfterOperation.Get(Context.ExecutionContext))
                base.ExecuteWorkflowLogic();
        }

        #endregion Overriddes 
    }
}
