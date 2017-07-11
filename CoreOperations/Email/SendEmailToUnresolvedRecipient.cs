using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.CoreOperations.Email
{
    public class SendEmailToUnresolvedRecipient : SendEmail
    {
        #region Inputs/Outputs

        [Input("Recipient Email")]
        [RequiredArgument]
        public InArgument<string> RecipientEmail { get; set; }

        [Input("Send Email")]
        [RequiredArgument]
        [Default("True")]
        public InArgument<bool> SendAfterOperation { get; set; }

        #endregion Inputs/Outputs

        #region Overriddes

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service,
            IOrganizationService sysService)
        {
            var emailId = Email.Get(executionContext).Id;

            var email = sysService.Retrieve("email", emailId, new ColumnSet("to"));

            var to = email.Contains("to") ? email.GetAttributeValue<EntityCollection>("to") : new EntityCollection();
            var newRecipient = new Entity("activityparty")
            {
                ["addressused"] = RecipientEmail.Get(executionContext)
            };
            to.Entities.Add(newRecipient);

            email["to"] = to;

            sysService.Update(email);

            if (SendAfterOperation.Get(executionContext))
                base.ExecuteWorkflowLogic(executionContext, context, service, sysService);
        }

        #endregion Overriddes

    }
}
