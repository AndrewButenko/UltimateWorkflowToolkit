using System.Linq;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Annotation
{
    public abstract class OperationBaseFile : CrmWorkflowBase
    {
        #region Inputs/Outputs

        [Input("Record Reference")]
        [RequiredArgument]
        public InArgument<string> Record { get; set; }

        [Input("Current File Name")]
        [RequiredArgument]
        public InArgument<string> CurrentFileName { get; set; }

        [Input("Resulting File Name")]
        [RequiredArgument]
        public InArgument<string> ResultingFileName { get; set; }

        #endregion Inputs/Outputs

        protected abstract void ExecuteOperation(Entity file, EntityReference parentRecord, string currentFileName, string resultingFileName);

        protected override void ExecuteWorkflowLogic()
        {
            var record = Record.Get(Context.ExecutionContext);

            if (string.IsNullOrEmpty(record))
                throw new InvalidPluginExecutionException("Record Reference Parameter is empty!");

            var currentFileName = CurrentFileName.Get(Context.ExecutionContext);

            if (string.IsNullOrEmpty(currentFileName))
                throw new InvalidPluginExecutionException("Current File Name Parameter is empty!");

            var resultingFileName = ResultingFileName.Get(Context.ExecutionContext);

            if (string.IsNullOrEmpty(resultingFileName))
                throw new InvalidPluginExecutionException("Resulting File Name Parameter is empty!");

            var recordReference = ConvertToEntityReference(record);

            var documentQuery = new QueryByAttribute("annotation")
            {
                ColumnSet = new ColumnSet("documentbody", "subject"),
                TopCount = 1
            };
            documentQuery.AddAttributeValue("objectid", recordReference.Id);
            documentQuery.AddAttributeValue("isdocument", true);
            documentQuery.AddAttributeValue("filename", currentFileName);

            var file = Context.SystemService.RetrieveMultiple(documentQuery).Entities.FirstOrDefault();

            if (file == null)
            {
                throw new InvalidPluginExecutionException($"File {currentFileName} is not available for current record");
            }

            ExecuteOperation(file, recordReference, currentFileName, resultingFileName);
        }
    }
}
