using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.CoreOperations.Annotation
{
    public class RenameFile : OperationBaseFile
    {
        #region Inputs/Outputs

        [Input("Resulting File Name")]
        [RequiredArgument]
        public InArgument<string> ResultingFileName { get; set; }

        #endregion Inputs/Outputs

        protected override void ExecuteOperation(Entity file, EntityReference parentRecord, string currentFileName)
        {
            var resultingFileName = ResultingFileName.Get(Context.ExecutionContext);

            if (string.IsNullOrEmpty(resultingFileName))
                throw new InvalidPluginExecutionException("Resulting File Name Parameter is empty!");

            file.Attributes.Clear();
            file["filename"] = resultingFileName;
            Context.SystemService.Update(file);
        }
    }
}
