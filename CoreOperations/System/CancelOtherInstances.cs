using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.System
{
    public class CancelOtherInstances : CrmWorkflowBase
    {
        protected override void ExecuteWorkflowLogic()
        {
            Entity asyncOperation = Context.SystemService.Retrieve("asyncoperation", Context.WorkflowExecutionContext.OperationId, new ColumnSet("workflowactivationid"));

            var asyncOperationsQuery = new QueryExpression("asyncoperation")
            {
                ColumnSet = new ColumnSet(false)
            };
            asyncOperationsQuery.Criteria.AddCondition("asyncoperationid", ConditionOperator.NotEqual, asyncOperation.Id);
            asyncOperationsQuery.Criteria.AddCondition("workflowactivationid", ConditionOperator.Equal,
                asyncOperation.GetAttributeValue<EntityReference>("workflowactivationid").Id);
            asyncOperationsQuery.Criteria.AddCondition("statecode", ConditionOperator.Equal, 1);
            asyncOperationsQuery.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, Context.WorkflowExecutionContext.PrimaryEntityId);

            var existingInstances = Context.SystemService.RetrieveMultiple(asyncOperationsQuery).Entities.ToList();


            foreach (Entity existingInstance in existingInstances)
            {
                existingInstance["statecode"] = new OptionSetValue(3);
                existingInstance["statuscode"] = new OptionSetValue(32);

                Context.SystemService.Update(existingInstance);
            }
        }
    }
}
