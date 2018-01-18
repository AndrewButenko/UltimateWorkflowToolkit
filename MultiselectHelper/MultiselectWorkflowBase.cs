using System;
using System.Activities;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.MultiselectHelper
{
    public abstract class MultiselectWorkflowBase : CrmWorkflowBase
    {

        #region Input/Output Parameters

        [Input("Record Reference")]
        [RequiredArgument]
        public InArgument<string> RecordReferenceString { get; set; }

        [Input("Field Name")]
        [RequiredArgument]
        public InArgument<string> FieldNameString { get; set; }

        #endregion Input/Output Parameters

        #region Protected Members

        protected Entity Record
        {
            get
            {
                if (Context == null || Context.ExecutionContext == null)
                    return null;

                var recordReference = ConvertToEntityReference(RecordReferenceString.Get(Context.ExecutionContext));

                return Context.UserService.Retrieve(recordReference.LogicalName, recordReference.Id, new ColumnSet(FieldName));
            }
        }

        protected string FieldName
        {
            get
            {
                if (Context?.ExecutionContext == null)
                    return null;

                return FieldNameString.Get(Context.ExecutionContext);
            }
        }

        protected List<int> ConvertStringToIntArray(string stringValues)
        {
            if (string.IsNullOrEmpty(stringValues))
                throw new InvalidPluginExecutionException("Parameter StringValues can't be empty!");

            var intValues = new List<int>();

            var stringValuesList = stringValues.Replace(" ", string.Empty).Split('|');

            foreach (var stringValue in stringValuesList)
            {
                int parseResult;

                if (!int.TryParse(stringValue, out parseResult))
                {
                    throw new InvalidPluginExecutionException($"{stringValue} can't be parsed as valid whole number");
                }

                intValues.Add(parseResult);
            }

            return intValues;
        }

        #endregion Protected Members

    }
}
