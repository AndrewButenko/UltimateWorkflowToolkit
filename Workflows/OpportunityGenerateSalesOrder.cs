using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public class OpportunityGenerateSalesOrder : CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Opportunity")]
        [ReferenceTarget("opportunity")]
        [RequiredArgument]
        public InArgument<EntityReference> Opportunity { get; set; }

        [Output("Sales Order")]
        [ReferenceTarget("salesorder")]
        public OutArgument<EntityReference> SalesOrder { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service)
        {
            var generateSalesOrderFromOpportunityRequest = new GenerateSalesOrderFromOpportunityRequest
            {
                OpportunityId = Opportunity.Get(executionContext).Id,
                ColumnSet = new ColumnSet("salesorderid")
            };

            var generateSalesOrderFromOpportunityResponse =
                (GenerateSalesOrderFromOpportunityResponse)service.Execute(generateSalesOrderFromOpportunityRequest);

            SalesOrder.Set(executionContext, generateSalesOrderFromOpportunityResponse.Entity.ToEntityReference());
        }
    }
}
