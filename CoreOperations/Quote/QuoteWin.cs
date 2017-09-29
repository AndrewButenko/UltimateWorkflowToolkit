using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class QuoteWin: CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Quote")]
        [ReferenceTarget("quote")]
        [RequiredArgument]
        public InArgument<EntityReference> Quote { get; set; }

        [Input("Quote Status")]
        [AttributeTarget("quote", "statuscode")]
        [RequiredArgument]
        public InArgument<OptionSetValue> QuoteStatus { get; set; }

        [Input("Quote Close: Subject")]
        public InArgument<string> Subject { get; set; }

        [Input("Quote Close: Close Date")]
        [RequiredArgument]
        public InArgument<DateTime> CloseDate { get; set; }

        [Input("Quote Close: Description")]
        public InArgument<string> Description { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic()
        {
            var winQuoteRecuest = new WinQuoteRequest()
            {
                Status = QuoteStatus.Get(Context.ExecutionContext),
                QuoteClose = new Entity("quoteclose")
                {
                    ["subject"] = Subject.Get(Context.ExecutionContext),
                    ["quoteid"] = Quote.Get(Context.ExecutionContext),
                    ["actualend"] = CloseDate.Get(Context.ExecutionContext),
                    ["description"] = Description.Get(Context.ExecutionContext)
                }
            };

            Context.UserService.Execute(winQuoteRecuest);
        }
    }
}
