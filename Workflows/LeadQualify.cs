using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
using Microsoft.Crm.Sdk.Messages;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class LeadQualify : CodeActivity
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

        protected override void Execute(CodeActivityContext executionContext)
        {
            var context = executionContext.GetExtension<IWorkflowContext>();
            var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            var service = serviceFactory.CreateOrganizationService(context.UserId);

            var qualifyLeadRequest = new QualifyLeadRequest()
            {
                CreateAccount = IsCreateAccont.Get(executionContext),
                CreateContact = IsCreateContact.Get(executionContext),
                CreateOpportunity = IsCreateOpportunity.Get(executionContext),
                LeadId = Lead.Get(executionContext),
                Status = LeadStatus.Get(executionContext),
                OpportunityCurrencyId = Currency.Get(executionContext)
            };

            if (OpportunityCustomerAccount.Get(executionContext) != null)
            {
                qualifyLeadRequest.OpportunityCustomerId = OpportunityCustomerAccount.Get(executionContext);
            }
            else if (OpportunityCustomerContact.Get(executionContext) != null)
            {
                qualifyLeadRequest.OpportunityCustomerId = OpportunityCustomerContact.Get(executionContext);
            }

            var qualifyLeadResponse = (QualifyLeadResponse) service.Execute(qualifyLeadRequest);

            foreach (var createdEntity in qualifyLeadResponse.CreatedEntities)
            {
                switch (createdEntity.LogicalName)
                {
                    case "account":
                        Account.Set(executionContext, createdEntity);
                        break;
                    case "contact":
                        Contact.Set(executionContext, createdEntity);
                        break;
                    case "opportunity":
                        Opportunity.Set(executionContext, createdEntity);
                        break;
                }
            }
            

        }
    }
}
