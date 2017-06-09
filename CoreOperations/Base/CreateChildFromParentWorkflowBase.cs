using System.Activities;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Base
{
    public abstract class CreateChildFromParentWorkflowBase: CrmWorkflowBase
    {
        #region Inputs

        [Input("Conditions (leave blank to copy all)")]
        public InArgument<string> Conditions { get; set; }

        #endregion Inputs

        #region Abstracts

        protected abstract EntityReference GetSourceEntity(CodeActivityContext executionContext);

        protected abstract void SetTargetEntity(CodeActivityContext executionContext, EntityReference target);

        protected abstract string SourceEntityChild { get; }

        protected abstract string SourceEntityLookupFieldName { get; }

        protected abstract string TargetEntity { get; }

        protected abstract string TargetEntityChild { get; }

        protected abstract string TargetEntityLookupFieldName { get; }

        #endregion Abstracts


        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service,
            IOrganizationService sysService)
        {
            var initializeFromRequest = new InitializeFromRequest()
            {
                EntityMoniker = GetSourceEntity(executionContext),
                TargetEntityName = TargetEntity,
                TargetFieldType = TargetFieldType.ValidForCreate
            };

            var initializeFromResponse = (InitializeFromResponse)service.Execute(initializeFromRequest);

            var targetEntityId = service.Create(initializeFromResponse.Entity);

            var target = new EntityReference(TargetEntity, targetEntityId);

            SetTargetEntity(executionContext, target);

            var additionalCondition = Conditions.Get(executionContext) ?? string.Empty;

            var sourceEntityFetchXml = $@"
                <fetch>
                  <entity name='{SourceEntityChild}' >
                    <attribute name='{SourceEntityChild}id' />
                    <filter type='and' >
                      <condition attribute='{SourceEntityLookupFieldName}' operator='eq' value='{GetSourceEntity(executionContext).Id}' />
                      {additionalCondition}
                    </filter>
                  </entity>
                </fetch>";

            var sourceEntityQuery = new FetchExpression(sourceEntityFetchXml);

            var sourceEntities = QueryWithPaging(sourceEntityQuery, service);

            foreach (var sourceEntity in sourceEntities)
            {
                initializeFromRequest = new InitializeFromRequest()
                {
                    EntityMoniker = sourceEntity.ToEntityReference(),
                    TargetEntityName = TargetEntityChild,
                    TargetFieldType = TargetFieldType.ValidForCreate
                };

                initializeFromResponse = (InitializeFromResponse)service.Execute(initializeFromRequest);

                var targetEntity = initializeFromResponse.Entity;
                targetEntity[TargetEntityLookupFieldName] = target;
                service.Create(targetEntity);
            }
        }
    }
}
