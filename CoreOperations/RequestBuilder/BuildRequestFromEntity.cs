using System;
using System.Activities;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.CoreOperations.Base;

namespace UltimateWorkflowToolkit.CoreOperations.RequestBuilder
{
    public class BuildRequestFromEntity: BuildRequestWorkflowBase
    {
        #region Inputs

        [Input("Source Entity")]
        [RequiredArgument]
        public InArgument<string> RecordId { get; set; }

        [Input("Source Entity Fields")]
        [RequiredArgument]
        public InArgument<string> SourceFields { get; set; }

        [Input("Destination Request Fields")]
        [RequiredArgument]
        public InArgument<string> DestinationFields { get; set; }

        #endregion Inputs

        protected override void BuildRequest(ref Dictionary<string, object> request)
        {
            var sourceFields = SourceFields.Get(Context.ExecutionContext)
                .Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries);
            var destinationFields = DestinationFields.Get(Context.ExecutionContext)
                .Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries);

            if (sourceFields.Length != destinationFields.Length)
                throw new InvalidPluginExecutionException("Number of Source Fields is not equal to number of Destination Fields");

            var entityReference = ConvertToEntityReference(RecordId.Get(Context.ExecutionContext));

            var sourceRecord = Context.SystemService.Retrieve(entityReference.LogicalName, entityReference.Id,
                new ColumnSet(sourceFields));

            for (var i = 0; i < sourceFields.Length; i++)
            {
                object targetFieldValue = null;

                if (sourceRecord.Contains(sourceFields[i]))
                    targetFieldValue = sourceRecord[sourceFields[i]];

                request.Add(destinationFields[i], targetFieldValue);
            }
        }
    }
}
