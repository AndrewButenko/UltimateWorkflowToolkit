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

        protected override void ExecuteWorkflowLogic()
        {
            var emailId = Email.Get(Context.ExecutionContext).Id;

            var email = Context.UserService.Retrieve("email", emailId, new ColumnSet("to"));

            var to = email.Contains("to") ? email.GetAttributeValue<EntityCollection>("to") : new EntityCollection();
            var newRecipient = new Entity("activityparty")
            {
                ["addressused"] = RecipientEmail.Get(Context.ExecutionContext)
            };
            to.Entities.Add(newRecipient);

            email["to"] = to;

            Context.UserService.Update(email);

            if (SendAfterOperation.Get(Context.ExecutionContext))
                base.ExecuteWorkflowLogic();
        }

        #endregion Overriddes

    }
}
