using Microsoft.Xrm.Sdk;

namespace UltimateWorkflowToolkit.CoreOperations.Annotation
{
    public class RenameFile : OperationBaseFile
    {
        protected override void ExecuteOperation(Entity file, EntityReference parentRecord, string currentFileName, string resultingFileName)
        {
            file.Attributes.Clear();
            file["filename"] = resultingFileName;
            Context.SystemService.Update(file);
        }
    }
}
