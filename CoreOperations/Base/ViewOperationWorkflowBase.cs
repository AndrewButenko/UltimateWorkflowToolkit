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

        protected abstract void ProcessRecords(List<Entity> records);

        #endregion Abstracts

        #region Overriddes

        protected override void ExecuteWorkflowLogic()
        {
            var publicView = PublicView.Get(Context.ExecutionContext);
            var privateView = PrivateView.Get(Context.ExecutionContext);
            var fetchXml = FetchXmlQuery.Get(Context.ExecutionContext);

            if (publicView == null &&
                privateView == null &&
                fetchXml == null)
                throw new InvalidPluginExecutionException("One of 'Public View', 'Private View' or 'Fetch Xml Query' inputs has to be populated!");

            if (publicView != null)
            {
                fetchXml = Context.SystemService.Retrieve(publicView.LogicalName, publicView.Id, new ColumnSet("fetchxml")).GetAttributeValue<string>("fetchxml");
            }
            else if (privateView != null)
            {
                fetchXml = Context.SystemService.Retrieve(privateView.LogicalName, privateView.Id, new ColumnSet("fetchxml")).GetAttributeValue<string>("fetchxml");
            }

            var allRecords = QueryWithPaging(new FetchExpression(fetchXml));

            ProcessRecords(allRecords);
        }

        #endregion Overriddes

    }
}
