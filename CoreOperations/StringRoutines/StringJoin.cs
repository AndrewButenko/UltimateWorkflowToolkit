using System.Linq;
using System.Activities;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.StringRoutines
{
    public class StringJoin: CrmWorkflowBase
    {
        #region Workflow Arguments

        [Input("In 1")]
        public InArgument<string> In1 { get; set; }

        [Input("In 2")]
        public InArgument<string> In2 { get; set; }

        [Input("In 3")]
        public InArgument<string> In3 { get; set; }

        [Input("In 4")]
        public InArgument<string> In4 { get; set; }

        [Input("In 5")]
        public InArgument<string> In5 { get; set; }

        [Input("In 6")]
        public InArgument<string> In6 { get; set; }

        [Input("In 7")]
        public InArgument<string> In7 { get; set; }

        [Input("In 8")]
        public InArgument<string> In8 { get; set; }

        [Input("In 9")]
        public InArgument<string> In9 { get; set; }

        [Input("In 10")]
        public InArgument<string> In10 { get; set; }

        [Input("Separator")]
        public InArgument<string> Separator { get; set; }

        [Output("Result")]
        public OutArgument<string> Result { get; set; }

        #endregion

        protected override void ExecuteWorkflowLogic()
        {
            var allStrings = new List<string>();

            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            properties.ToList().ForEach(p =>
            {
                if (p.PropertyType.IsSubclassOf(typeof(InArgument)) &&
                    p.Name.StartsWith("In"))
                {
                    var property = (Argument)p.GetValue(this);
                    var propertyValue = property.Get(Context.ExecutionContext);

                    if (propertyValue != null)
                    {
                        allStrings.Add((string)propertyValue);
                    }
                }
            });

            var separator = Separator.Get(Context.ExecutionContext) ?? string.Empty;

            var result = string.Join(separator, allStrings);

            Result.Set(Context.ExecutionContext, result);
        }
    }
}
