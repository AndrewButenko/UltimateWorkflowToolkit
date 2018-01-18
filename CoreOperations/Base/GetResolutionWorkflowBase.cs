using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Base
{
    public abstract class GetResolutionWorkflowBase : CrmWorkflowBase
    {
        #region Abstract Properties

        protected abstract Guid ParentRecordId { get; }
        protected abstract string ResolutionEntityName { get; }
        protected abstract string ParentRecordLookupFieldName { get; }
        protected abstract void SetResolutionEntity(EntityReference resolution);

        #endregion Abstract Properties

        protected override void ExecuteWorkflowLogic()
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
            query.AddAttributeValue(ParentRecordLookupFieldName, ParentRecordId);
            query.AddAttributeValue("statecode", 1);

            var resolutionEntity = Context.SystemService.RetrieveMultiple(query).Entities.FirstOrDefault();

            SetResolutionEntity(resolutionEntity?.ToEntityReference());
        }
    }

}
