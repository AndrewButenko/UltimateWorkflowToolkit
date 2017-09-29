using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
using Microsoft.Crm.Sdk.Messages;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class OpportunityWin : CrmWorkflowBase
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

        protected override void ExecuteWorkflowLogic()
        {
            var opportunityClose = new Entity("opportunityclose")
            {
                ["subject"] = Subject.Get(Context.ExecutionContext),
                ["opportunityid"] = Opportunity.Get(Context.ExecutionContext),
                ["actualrevenue"] = ActualRevenue.Get(Context.ExecutionContext),
                ["actualend"] = CloseDate.Get(Context.ExecutionContext),
                ["description"] = Description.Get(Context.ExecutionContext)
            };

            var winOpportunityRequest = new WinOpportunityRequest()
            {
                Status = OpportunityStatus.Get(Context.ExecutionContext),
                OpportunityClose = opportunityClose
            };

            Context.UserService.Execute(winOpportunityRequest);
        }
    }
}
