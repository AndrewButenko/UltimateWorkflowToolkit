using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.System
{
    public class RecalculateRollup : CrmWorkflowBase
    {
        #region Input/Output Arguments

        [Input("Record Reference")]
        [RequiredArgument]
        public InArgument<string> Record { get; set; }

        [Input("Rollup Field Name")]
        [RequiredArgument]
        public InArgument<string> FieldName { get; set; }

        #endregion Input/Output Arguments

        #region Overriddes

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service, IOrganizationService sysService)
        {
            var target = ConvertToEntityReference(Record.Get(executionContext), service);

            sysService.Execute(new CalculateRollupFieldRequest()
            {
                FieldName = FieldName.Get(executionContext),
                Target = target
            });
        }

        #endregion Overriddes

    }
}
