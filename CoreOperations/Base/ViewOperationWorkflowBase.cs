using System.Activities;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Base
{
    public abstract class ViewOperationWorkflowBase : CrmWorkflowBase
    {
        #region Input/Output Arguments

        [Input("Public View")]
        [ReferenceTarget("savedquery")]
        public InArgument<EntityReference> PublicView { get; set; }

        [Input("Private View")]
        [ReferenceTarget("userquery")]
        public InArgument<EntityReference> PrivateView { get; set; }

        [Input("FetchXml Query")]
        public InArgument<string> FetchXmlQuery { get; set; }

        #endregion Input/Output Arguments

        #region Abstracts

        protected abstract void ProcessRecords(List<Entity> records, CodeActivityContext executionContext, IOrganizationService service, IOrganizationService sysService);

        #endregion Abstracts

        #region Overriddes

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service, IOrganizationService sysService)
        {
            var publicView = PublicView.Get(executionContext);
            var privateView = PrivateView.Get(executionContext);
            var fetchXml = FetchXmlQuery.Get(executionContext);

            if (publicView == null &&
                privateView == null &&
                fetchXml == null)
                throw new InvalidPluginExecutionException("One of 'Public View', 'Private View' or 'Fetch Xml Query' inputs has to be populated!");

            if (publicView != null)
            {
                fetchXml = sysService.Retrieve(publicView.LogicalName, publicView.Id, new ColumnSet("fetchxml")).GetAttributeValue<string>("fetchxml");
            }
            else if (privateView != null)
            {
                fetchXml = sysService.Retrieve(privateView.LogicalName, privateView.Id, new ColumnSet("fetchxml")).GetAttributeValue<string>("fetchxml");
            }

            var allRecords = QueryWithPaging(new FetchExpression(fetchXml), sysService);

            ProcessRecords(allRecords, executionContext, service, sysService);
        }

        #endregion Overriddes

    }
}
