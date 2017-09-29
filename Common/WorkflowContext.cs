using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateWorkflowToolkit.Common
{
    public class WorkflowContext
    {
        #region Properties

        public CodeActivityContext ExecutionContext
        {
            get;
            private set;
        }

        public IWorkflowContext WorkflowExecutionContext
        {
            get;
            private set;
        }

        public IOrganizationService UserService
        {
            get;
            private set;
        }

        public IOrganizationService SystemService
        {
            get;
            private set;
        }

        public ITracingService TracingService
        {
            get;
            private set;
        }

        #endregion Properties

        #region CTOR

        private WorkflowContext()
        {
            throw new Exception("Don't user default CTOR");
        }

        public WorkflowContext(CodeActivityContext executionContext)
        {
            ExecutionContext = executionContext;
            WorkflowExecutionContext = executionContext.GetExtension<IWorkflowContext>();

            var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            UserService = serviceFactory.CreateOrganizationService(WorkflowExecutionContext.UserId);
            SystemService = serviceFactory.CreateOrganizationService(null);
            TracingService = executionContext.GetExtension<ITracingService>();
        }

        #endregion CTOR
    }
}
