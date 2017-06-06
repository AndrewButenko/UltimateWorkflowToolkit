using System;
using System.Linq;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Base
{
    public abstract class GetResolutionWorkflowBase : CrmWorkflowBase
    {
        #region Abstract Properties

        protected abstract Guid GetParentRecordId(CodeActivityContext executionContext);
        protected abstract string ResolutionEntityName { get; }
        protected abstract string ParentRecordLookupFieldName { get; }
        protected abstract void SetResolutionEntity(CodeActivityContext executionContext, EntityReference resolution);

        #endregion Abstract Properties

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service, IOrganizationService sysService)
        {
            var query = new QueryByAttribute(ResolutionEntityName)
            {
                ColumnSet = new ColumnSet(false),
                PageInfo = new PagingInfo()
                {
                    Count = 1,
                    PageNumber = 1
                }
            };
            query.AddAttributeValue(ParentRecordLookupFieldName, GetParentRecordId(executionContext));
            query.AddAttributeValue("statecode", 1);

            var resolutionEntity = service.RetrieveMultiple(query).Entities.FirstOrDefault();

            SetResolutionEntity(executionContext, resolutionEntity?.ToEntityReference());
        }
    }

}
