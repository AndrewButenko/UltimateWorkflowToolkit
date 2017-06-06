using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Base
{
    public abstract class AddDetailWorkflowBase : CrmWorkflowBase
    {
        #region Inputs

        [Input("Write-in Product")]
        public InArgument<string> ProductName { get; set; }

        [Input("Existing Product")]
        [ReferenceTarget("product")]
        public InArgument<EntityReference> ProductId { get; set; }

        [Input("Unit")]
        [ReferenceTarget("uom")]
        public InArgument<EntityReference> UomId { get; set; }

        [Input("Price Overridden")]
        public InArgument<bool> IsPriceOverridden { get; set; }

        [Input("Price Per Unit")]
        public InArgument<Money> PricePerUnit { get; set; }

        [Input("Quantity")]
        [RequiredArgument]
        public InArgument<decimal> Quantity { get; set; }

        [Input("Manual Discount")]
        public InArgument<Money> ManualDiscountAmount { get; set; }

        [Input("Tax")]
        public InArgument<Money> Tax { get; set; }

        [Input("Description")]
        public InArgument<string> Description { get; set; }

        #endregion Inputs

        #region Abstracts

        protected abstract string ProductEntityName { get; }
        protected abstract string ParentEntityLookupFieldName { get; }
        protected abstract EntityReference GetParentEntity(CodeActivityContext executionContext);
        protected abstract void ProcessAdditionalFields(ref Entity record, CodeActivityContext executionContext);

        #endregion Abstracts

        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context,
            IOrganizationService service, IOrganizationService sysService)
        {
            var detailRecord = new Entity(ProductEntityName)
            {
                [ParentEntityLookupFieldName] = GetParentEntity(executionContext),
                ["quantity"] = Quantity.Get(executionContext),
                ["description"] = Description.Get(executionContext),
                ["manualdiscountamount"] = ManualDiscountAmount.Get(executionContext),
                ["tax"] = Tax.Get(executionContext)
            };

            var writeInProduct = ProductName.Get(executionContext);

            if (!string.IsNullOrEmpty(writeInProduct))
            {
                detailRecord["productdescription"] = writeInProduct;
                detailRecord["isproductoverridden"] = true;
                detailRecord["ispriceoverridden"] = true;
                detailRecord["priceperunit"] = PricePerUnit.Get(executionContext);
            }
            else
            {
                detailRecord["productid"] = ProductId.Get(executionContext);
                detailRecord["isproductoverridden"] = false;
                detailRecord["uomid"] = UomId.Get(executionContext);

                var ispriceoverridden = IsPriceOverridden.Get(executionContext);
                detailRecord["ispriceoverridden"] = ispriceoverridden;

                if (ispriceoverridden)
                {
                    detailRecord["priceperunit"] = PricePerUnit.Get(executionContext);
                }
            }

            ProcessAdditionalFields(ref detailRecord, executionContext);

            service.Create(detailRecord);
        }
    }
}
