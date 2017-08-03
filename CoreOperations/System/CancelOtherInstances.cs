using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;
using Microsoft.Xrm.Sdk.Query;

namespace UltimateWorkflowToolkit.CoreOperations.System
{
    public class CancelOtherInstances : CrmWorkflowBase
    {

        #region Overriddes

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service, IOrganizationService sysService)
        {
            Entity asyncOperation = sysService.Retrieve("asyncoperation", context.OperationId, new ColumnSet("workflowactivationid"));

            var asyncOperationsQuery = new QueryExpression("asyncoperation")
            {
                ColumnSet = new ColumnSet(false)
            };
            asyncOperationsQuery.Criteria.AddCondition("asyncoperationid", ConditionOperator.NotEqual, asyncOperation.Id);
            asyncOperationsQuery.Criteria.AddCondition("workflowactivationid", ConditionOperator.Equal,
                asyncOperation.GetAttributeValue<EntityReference>("workflowactivationid").Id);
            asyncOperationsQuery.Criteria.AddCondition("statecode", ConditionOperator.Equal, 1);
            asyncOperationsQuery.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, context.PrimaryEntityId);

            var existingInstances = sysService.RetrieveMultiple(asyncOperationsQuery).Entities.ToList();


            foreach (Entity existingInstance in existingInstances)
            {
                existingInstance["statecode"] = new OptionSetValue(3);
                existingInstance["statuscode"] = new OptionSetValue(32);

                sysService.Update(existingInstance);
            }
        }

        #endregion Overriddes

    }
}
