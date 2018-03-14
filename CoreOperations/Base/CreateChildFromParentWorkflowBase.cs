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

        protected abstract EntityReference SourceEntity { get; }
        protected abstract void SetTargetEntity(EntityReference target);
        protected abstract string SourceEntityChild { get; }
        protected abstract string SourceEntityLookupFieldName { get; }
        protected abstract string TargetEntity { get; }
        protected abstract string TargetEntityChild { get; }
        protected abstract string TargetEntityLookupFieldName { get; }

        protected override void ExecuteWorkflowLogic()
        {
            var initializeFromRequest = new InitializeFromRequest()
            {
                EntityMoniker = SourceEntity,
                TargetEntityName = TargetEntity,
                TargetFieldType = TargetFieldType.ValidForCreate
            };

            var initializeFromResponse = (InitializeFromResponse)Context.UserService.Execute(initializeFromRequest);

            var targetEntityId = Context.UserService.Create(initializeFromResponse.Entity);

            var target = new EntityReference(TargetEntity, targetEntityId);

            SetTargetEntity(target);

            var additionalCondition = Conditions.Get(Context.ExecutionContext) ?? string.Empty;

            var sourceEntityFetchXml = $@"
                <fetch>
                  <entity name='{SourceEntityChild}' >
                    <attribute name='{SourceEntityChild}id' />
                    <filter type='and' >
                      <condition attribute='{SourceEntityLookupFieldName}' operator='eq' value='{SourceEntity.Id}' />
                      {additionalCondition}
                    </filter>
                  </entity>
                </fetch>";

            var sourceEntityQuery = new FetchExpression(sourceEntityFetchXml);

            var sourceEntities = QueryWithPaging(sourceEntityQuery);

            foreach (var sourceEntity in sourceEntities)
            {
                initializeFromRequest = new InitializeFromRequest()
                {
                    EntityMoniker = sourceEntity.ToEntityReference(),
                    TargetEntityName = TargetEntityChild,
                    TargetFieldType = TargetFieldType.ValidForCreate
                };

                initializeFromResponse = (InitializeFromResponse)Context.UserService.Execute(initializeFromRequest);

                var targetEntity = initializeFromResponse.Entity;
                targetEntity[TargetEntityLookupFieldName] = target;
                Context.UserService.Create(targetEntity);
            }
        }
    }
}
