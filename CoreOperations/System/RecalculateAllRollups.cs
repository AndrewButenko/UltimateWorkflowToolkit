using System.Linq;
using System.Activities;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.System
{
    public class RecalculateAllRollups : CrmWorkflowBase
    {
        #region Input/Output Arguments

        [Input("Record Reference")]
        [RequiredArgument]
        public InArgument<string> Record { get; set; }

        #endregion Input/Output Arguments

        protected override void ExecuteWorkflowLogic()
        {
            var target = ConvertToEntityReference(Record.Get(Context.ExecutionContext));

            var retrieveEntityResponse = (RetrieveEntityResponse)Context.SystemService.Execute(new RetrieveEntityRequest()
            {
                EntityFilters = EntityFilters.Attributes,
                LogicalName = target.LogicalName,
                RetrieveAsIfPublished = true
            });

            var entityMetadata = retrieveEntityResponse.EntityMetadata;

            entityMetadata.Attributes.Where(a => a.SourceType == 2 && (a.GetType() != typeof(MoneyAttributeMetadata) || (a is MoneyAttributeMetadata && ((MoneyAttributeMetadata)a).CalculationOf == null))).Select(a => a.LogicalName).ToList().ForEach(fieldName => {
                Context.SystemService.Execute(new CalculateRollupFieldRequest()
                {
                    FieldName = fieldName,
                    Target = target
                });
            });
        }
    }
}
