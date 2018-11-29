using System;
using System.Activities;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Crm.Sdk.Messages;
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

        [Output("Duplicates Table")]
        public OutArgument<string> DuplicatesTable { get; set; }

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

            var urlsResult = string.Empty;
            var tableResult = "<table>";

            var entities = new Dictionary<string, EntityMetadata>();

            foreach (var dupRecord in retrieveDuplicatesResponse.DuplicateCollection.Entities)
            {
                var recordUrl =
                    $"{Context.Settings.BaseUrl}/main.aspx?etn={dupRecord.LogicalName}&pagetype=entityrecord&id={dupRecord.Id}";

                urlsResult = urlsResult + (urlsResult == string.Empty ? string.Empty : Environment.NewLine) + recordUrl;

                if (!entities.ContainsKey(dupRecord.LogicalName))
                {
                    var entityMetadata =
                        ((RetrieveEntityResponse) Context.SystemService.Execute(new RetrieveEntityRequest()
                        {
                            EntityFilters = EntityFilters.Entity, 
                            LogicalName = dupRecord.LogicalName,
                            RetrieveAsIfPublished = true
                        })).EntityMetadata;

                    entities.Add(dupRecord.LogicalName, entityMetadata);
                }

                var displayText =
                    dupRecord.GetAttributeValue<string>(entities[dupRecord.LogicalName].PrimaryNameAttribute) ?? "Not Available";

                tableResult += $"<tr><td><a href='{recordUrl}'>{displayText}</a></td><tr>";
            }

            tableResult += "</table>";

            DuplicatesReferences.Set(Context.ExecutionContext, urlsResult);
            DuplicatesTable.Set(Context.ExecutionContext, tableResult);
        }
    }
}
