using Microsoft.Xrm.Sdk;

namespace UltimateWorkflowToolkit.CoreOperations.Annotation
{
    public class DeleteFile: OperationBaseFile
    {
        protected override void ExecuteOperation(Entity file, EntityReference parentRecord, string currentFileName)
        {
            Context.SystemService.Delete("annotation", file.Id);
        }
    }
}
