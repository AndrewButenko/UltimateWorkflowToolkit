using System.Linq;
using System.Activities;
using Newtonsoft.Json;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

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

        public UWTSettings Settings
        {
            get;
            private set;
        }

        #endregion Properties

        #region CTOR

        public WorkflowContext(CodeActivityContext executionContext)
        {
            ExecutionContext = executionContext;
            WorkflowExecutionContext = executionContext.GetExtension<IWorkflowContext>();

            var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            UserService = serviceFactory.CreateOrganizationService(WorkflowExecutionContext.UserId);
            SystemService = serviceFactory.CreateOrganizationService(null);
            TracingService = executionContext.GetExtension<ITracingService>();

            var settingsQuery = new QueryExpression("uwt_settings")
            {
                ColumnSet = new ColumnSet("uwt_settingsstring"),
                TopCount = 1
            };

            var settingsRecord = SystemService.RetrieveMultiple(settingsQuery).Entities.FirstOrDefault();

            if (settingsRecord != null && settingsRecord.Contains("uwt_settingsstring"))
            {
                try
                {
                    Settings =
                        JsonConvert.DeserializeObject<UWTSettings>(
                            settingsRecord.GetAttributeValue<string>("uwt_settingsstring"));
                }
                catch
                {
                }
            }
        }

        #endregion CTOR
    }

    public class UWTSettings
    {
        public string BingMapsKey { get; set; }
        public string CloudConvertKey { get; set; }
        public string CurrencylayerKey { get; set; }
    }
}
