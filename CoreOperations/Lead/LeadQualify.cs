using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
using Microsoft.Crm.Sdk.Messages;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class LeadQualify : CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Lead")]
        [ReferenceTarget("lead")]
        [RequiredArgument]
        public InArgument<EntityReference> Lead { get; set; }

        [Input("Lead Status")]
        [AttributeTarget("lead", "statuscode")]
        [RequiredArgument]
        public InArgument<OptionSetValue> LeadStatus { get; set; }

        [Input("Create Account")]
        [RequiredArgument]
        public InArgument<bool> IsCreateAccont { get; set; }

        [Output("Created Account")]
        [ReferenceTarget("account")]
        public OutArgument<EntityReference> Account { get; set; }

        [Input("Create Contact")]
        [RequiredArgument]
        public InArgument<bool> IsCreateContact { get; set; }

        [Output("Created Contact")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> Contact { get; set; }

        [Input("Create Opportunity")]
        [RequiredArgument]
        public InArgument<bool> IsCreateOpportunity { get; set; }

        [Output("Created Opportunity")]
        [ReferenceTarget("opportunity")]
        public OutArgument<EntityReference> Opportunity { get; set; }

        [Input("Currency")]
        [ReferenceTarget("transactioncurrency")]
        public InArgument<EntityReference> Currency { get; set; }

        [Input("Opportunity Customer(Account)")]
        [ReferenceTarget("account")]
        public InArgument<EntityReference> OpportunityCustomerAccount { get; set; }

        [Input("Opportunity Customer(Contact)")]
        [ReferenceTarget("contact")]
        public InArgument<EntityReference> OpportunityCustomerContact { get; set; }

        #endregion

        protected override void ExecuteWorkflowLogic()
        {
            var qualifyLeadRequest = new QualifyLeadRequest()
            {
                CreateAccount = IsCreateAccont.Get(Context.ExecutionContext),
                CreateContact = IsCreateContact.Get(Context.ExecutionContext),
                CreateOpportunity = IsCreateOpportunity.Get(Context.ExecutionContext),
                LeadId = Lead.Get(Context.ExecutionContext),
                Status = LeadStatus.Get(Context.ExecutionContext),
                OpportunityCurrencyId = Currency.Get(Context.ExecutionContext)
            };

            if (OpportunityCustomerAccount.Get(Context.ExecutionContext) != null)
            {
                qualifyLeadRequest.OpportunityCustomerId = OpportunityCustomerAccount.Get(Context.ExecutionContext);
            }
            else if (OpportunityCustomerContact.Get(Context.ExecutionContext) != null)
            {
                qualifyLeadRequest.OpportunityCustomerId = OpportunityCustomerContact.Get(Context.ExecutionContext);
            }

            var qualifyLeadResponse = (QualifyLeadResponse)Context.UserService.Execute(qualifyLeadRequest);

            foreach (var createdEntity in qualifyLeadResponse.CreatedEntities)
            {
                switch (createdEntity.LogicalName)
                {
                    case "account":
                        Account.Set(Context.ExecutionContext, createdEntity);
                        break;
                    case "contact":
                        Contact.Set(Context.ExecutionContext, createdEntity);
                        break;
                    case "opportunity":
                        Opportunity.Set(Context.ExecutionContext, createdEntity);
                        break;
                }
            }
        }
    }
}
