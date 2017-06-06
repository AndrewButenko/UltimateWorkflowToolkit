using System.Activities;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Base
{
    public abstract class CopyDetailsWorkflowsBase : CrmWorkflowBase
    {
        #region Inputs

        [Input("Conditions (leave blank to copy all)")]
        public InArgument<string> Conditions { get; set; }

        #endregion Inputs

        #region Abstracts

        protected abstract EntityReference GetSourceEntityParent(CodeActivityContext executionContext);

        protected abstract EntityReference GetTargetEntityParent(CodeActivityContext executionContext);

        protected abstract string SourceEntity { get; }

        protected abstract string SourceEntityLookupFieldName { get; }

        protected abstract string TargetEntity { get; }

        protected abstract string TargetEntityLookupFieldName { get; }

        #endregion Abstracts

        #region Overriddes

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service,
            IOrganizationService sysService)
        {
            var additionalCondition = Conditions.Get(executionContext) ?? string.Empty;

            var sourceEntityFetchXml = $@"
                <fetch>
                  <entity name='{SourceEntity}' >
                    <attribute name='{SourceEntity}id' />
                    <filter type='and' >
                      <condition attribute='{SourceEntityLookupFieldName}' operator='eq' value='{GetSourceEntityParent(executionContext).Id}' />
                      {additionalCondition}
                    </filter>
                  </entity>
                </fetch>";

            var sourceEntityQuery = new FetchExpression(sourceEntityFetchXml);

            var sourceEntities = QueryWithPaging(sourceEntityQuery, service);

            foreach (var sourceEntity in sourceEntities)
            {
                var initializeFromRequest = new InitializeFromRequest()
                {
                    EntityMoniker = sourceEntity.ToEntityReference(),
                    TargetEntityName = TargetEntity,
                    TargetFieldType = TargetFieldType.ValidForCreate
                };

                var initializeFromResponse = (InitializeFromResponse) service.Execute(initializeFromRequest);

                var targetEntity = initializeFromResponse.Entity;
                targetEntity[TargetEntityLookupFieldName] = GetTargetEntityParent(executionContext);
                service.Create(targetEntity);
            }
        }

        #endregion Overriddes

    }
}
