using System;
using System.Activities;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Common
{
    public class CheckForDuplicates: CrmWorkflowBase
    {
        #region Input/Output Arguments

        [Input("Record Reference")]
        [RequiredArgument]
        public InArgument<string> Record { get; set; }

        [Output("Duplicates Exist")]
        public  OutArgument<bool> IsDuplicatesExist { get; set; }

        [Output("Duplicates References")]
        public OutArgument<string> DuplicatesReferences { get; set; }

        #endregion Input/Output Arguments

        protected override void ExecuteWorkflowLogic()
        {
            var target = ConvertToEntityReference(Record.Get(Context.ExecutionContext));

            var targetRecord = Context.SystemService.Retrieve(target.LogicalName, target.Id, new ColumnSet(true));

            var retrieveDuplicatesRequest = new RetrieveDuplicatesRequest()
            {
                BusinessEntity = targetRecord,
                MatchingEntityName = target.LogicalName,
                PagingInfo = new PagingInfo()
                {
                    Count = 5000,
                    PageNumber = 1
                }
            };

            var retrieveDuplicatesResponse =
                (RetrieveDuplicatesResponse) Context.SystemService.Execute(retrieveDuplicatesRequest);

            var dupsExist = retrieveDuplicatesResponse.DuplicateCollection.Entities.Count != 0;

            IsDuplicatesExist.Set(Context.ExecutionContext, dupsExist);

            if (!dupsExist)
                return;

            var baseUrl = GetBaseUrl(Record.Get(Context.ExecutionContext));

            var urlsResult = string.Empty;

            foreach (var dupRecord in retrieveDuplicatesResponse.DuplicateCollection.Entities)
            {
                urlsResult = urlsResult + (urlsResult == string.Empty ? string.Empty : Environment.NewLine) +
                             baseUrl +
                             string.Format("/main.aspx?etn={0}&pagetype=entityrecord&id={1}", dupRecord.LogicalName,
                                 dupRecord.Id);
            }

            DuplicatesReferences.Set(Context.ExecutionContext, urlsResult);
        }
    }
}
