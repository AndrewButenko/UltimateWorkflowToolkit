using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
using Microsoft.Crm.Sdk.Messages;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class OpportunityWin : CodeActivity
    {
        #region Input/Output Parameters

        [Input("Opportunity")]
        [ReferenceTarget("opportunity")]
        [RequiredArgument]
        public InArgument<EntityReference> Opportunity { get; set; }

        [Input("Opportunity Status")]
        [AttributeTarget("opportunity", "statuscode")]
        [RequiredArgument]
        public InArgument<OptionSetValue> OpportunityStatus { get; set; }

        [Input("Opportunity Close: Subject")]
        public InArgument<string> Subject { get; set; }

        [Input("Opportunity Close: Actual Revenue")]
        [RequiredArgument]
        public InArgument<Money> ActualRevenue { get; set; }

        [Input("Opportunity Close: Close Date")]
        [RequiredArgument]
        public InArgument<DateTime> CloseDate { get; set; }

        [Input("Opportunity Close: Description")]
        public InArgument<string> Description { get; set; }

        #endregion Input/Output Parameters

        protected override void Execute(CodeActivityContext executionContext)
        {
            var context = executionContext.GetExtension<IWorkflowContext>();
            var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            var service = serviceFactory.CreateOrganizationService(context.UserId);

            var opportunityClose = new Entity("opportunityclose")
            {
                ["subject"] = Subject.Get(executionContext),
                ["opportunityid"] = Opportunity.Get(executionContext),
                ["actualrevenue"] = ActualRevenue.Get(executionContext),
                ["actualend"] = CloseDate.Get(executionContext),
                ["description"] = Description.Get(executionContext)
            };

            var winOpportunityRequest = new WinOpportunityRequest()
            {
                Status = OpportunityStatus.Get(executionContext),
                OpportunityClose = opportunityClose
            };

            service.Execute(winOpportunityRequest);
        }
    }
}
