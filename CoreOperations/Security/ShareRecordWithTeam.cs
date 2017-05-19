using System.Activities;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Security
{
    public class ShareRecordWithTeam : CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("Record Reference")]
        [RequiredArgument]
        public InArgument<string> Record { get; set; }

        [Input("Team")]
        [RequiredArgument]
        [ReferenceTarget("team")]
        public InArgument<EntityReference> Team { get; set; }

        [Input("Read Access")]
        [RequiredArgument]
        [Default("False")]
        public InArgument<bool> ReadAccess { get; set; }

        [Input("Write Access")]
        [RequiredArgument]
        [Default("False")]
        public InArgument<bool> WriteAccess { get; set; }

        [Input("Delete Access")]
        [RequiredArgument]
        [Default("False")]
        public InArgument<bool> DeleteAccess { get; set; }

        [Input("Append Access")]
        [RequiredArgument]
        [Default("False")]
        public InArgument<bool> AppendAccess { get; set; }

        [Input("Append To Access")]
        [RequiredArgument]
        [Default("False")]
        public InArgument<bool> AppendToAccess { get; set; }

        [Input("Assign Access")]
        [RequiredArgument]
        [Default("False")]
        public InArgument<bool> AssignAccess { get; set; }

        [Input("Share Access")]
        [RequiredArgument]
        [Default("False")]
        public InArgument<bool> ShareAccess { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context,
            IOrganizationService service, IOrganizationService sysService)
        {
            var target = ConvertToEntityReference(Record.Get(executionContext), service);

            #region Build Sharing Mask

            var rights = AccessRights.None;

            if (ReadAccess.Get(executionContext))
            {
                rights |= AccessRights.ReadAccess;
            }

            if (WriteAccess.Get(executionContext))
            {
                rights |= AccessRights.WriteAccess;
            }

            if (DeleteAccess.Get(executionContext))
            {
                rights |= AccessRights.DeleteAccess;
            }

            if (AppendAccess.Get(executionContext))
            {
                rights |= AccessRights.AppendAccess;
            }

            if (AppendToAccess.Get(executionContext))
            {
                rights |= AccessRights.AppendToAccess;
            }

            if (AssignAccess.Get(executionContext))
            {
                rights |= AccessRights.AssignAccess;
            }

            if (ShareAccess.Get(executionContext))
            {
                rights |= AccessRights.ShareAccess;
            }

            #endregion Build Sharing Mask

            sysService.Execute(new GrantAccessRequest()
            {
                PrincipalAccess = new PrincipalAccess()
                {
                    AccessMask = rights,
                    Principal = Team.Get(executionContext)
                },
                Target = target
            });
        }
    }
}
