using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Linq;
using Microsoft.Xrm.Sdk.Query;

namespace UltimateWorkflowToolkit.CoreOperations
{
    public abstract class CrmWorkflowBase : CodeActivity
    {
        #region Input/Output Parameters

        [Input("Throw an Exception on Error")]
        [RequiredArgument]
        [Default("True")]
        public InArgument<bool> IsThrowException { get; set; }

        [Output("Error Occured")]
        public OutArgument<bool> IsExceptionOccured { get; set; }

        [Output("Error Message")]
        public OutArgument<string> ErrorMessage { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext executionContext)
        {
            var context = executionContext.GetExtension<IWorkflowContext>();
            var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            var service = serviceFactory.CreateOrganizationService(context.UserId);

            //ToDo: Include validation of InArguments that are marked as required

            try
            {
                ExecuteWorkflowLogic(executionContext, context, service);

                IsExceptionOccured.Set(executionContext, false);
            }
            catch (Exception e)
            {
                if (IsThrowException.Get(executionContext) || context.WorkflowMode == (int)WorkflowExecutionMode.RealTime)
                    throw;

                IsExceptionOccured.Set(executionContext, true);
                ErrorMessage.Set(executionContext, e.Message);
            }
        }

        protected abstract void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context,
            IOrganizationService service);
    }

    public abstract class GetResolutionWorkflowBase : CrmWorkflowBase
    {
        #region Abstract Properties

        protected abstract Guid GetParentRecordId(CodeActivityContext executionContext);
        protected abstract string ResolutionEntityName { get; }
        protected abstract string ParentRecordLookupFieldName { get; }
        protected abstract void SetResolutionEntity(CodeActivityContext executionContext, EntityReference resolution);

        #endregion Abstract Properties

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service)
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

    internal enum WorkflowExecutionMode : int
    {
        Asynchronous = 0,
        RealTime = 1
    }
}
