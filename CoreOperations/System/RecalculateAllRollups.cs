using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        #region Overriddes

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service, IOrganizationService sysService)
        {
            var target = ConvertToEntityReference(Record.Get(executionContext), service);

            var retrieveEntityResponse = (RetrieveEntityResponse)sysService.Execute(new RetrieveEntityRequest()
            {
                EntityFilters = Microsoft.Xrm.Sdk.Metadata.EntityFilters.Attributes,
                LogicalName = target.LogicalName,
                RetrieveAsIfPublished = true
            });

            var entityMetadata = retrieveEntityResponse.EntityMetadata;

            entityMetadata.Attributes.Where(a => a.SourceType == 2 && (a.GetType() != typeof(MoneyAttributeMetadata) || (a.GetType() == typeof(MoneyAttributeMetadata) && ((MoneyAttributeMetadata)a).CalculationOf == null))).Select(a => a.LogicalName).ToList().ForEach(fieldName => {
                sysService.Execute(new CalculateRollupFieldRequest()
                {
                    FieldName = fieldName,
                    Target = target
                });
            });
        }

        #endregion Overriddes

    }
}
