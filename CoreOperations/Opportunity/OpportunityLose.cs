using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using Microsoft.Crm.Sdk.Messages;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class OpportunityLose : CrmWorkflowBase
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

        [Input("Opportunity Close: Competitor")]
        [ReferenceTarget("competitor")]
        public InArgument<EntityReference> Competitor { get; set; }

        [Input("Opportunity Close: Description")]
        public InArgument<string> Description { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service, IOrganizationService sysService)
        {
            var opportunityClose = new Entity("opportunityclose")
            {
                ["subject"] = Subject.Get(executionContext),
                ["opportunityid"] = Opportunity.Get(executionContext),
                ["actualrevenue"] = ActualRevenue.Get(executionContext),
                ["actualend"] = CloseDate.Get(executionContext),
                ["competitorid"] = Competitor.Get(executionContext),
                ["description"] = Description.Get(executionContext)
            };

            var loseOpportunityRequest = new LoseOpportunityRequest()
            {
                Status = OpportunityStatus.Get(executionContext),
                OpportunityClose = opportunityClose
            };

            service.Execute(loseOpportunityRequest);
        }
    }
}
