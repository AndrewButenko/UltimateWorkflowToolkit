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

        protected abstract string ProductEntityName { get; }
        protected abstract string ParentEntityLookupFieldName { get; }
        protected abstract EntityReference ParentEntity { get; }
        protected abstract void ProcessAdditionalFields(ref Entity record);

        protected override void ExecuteWorkflowLogic()
        {
            var detailRecord = new Entity(ProductEntityName)
            {
                [ParentEntityLookupFieldName] = ParentEntity,
                ["quantity"] = Quantity.Get(Context.ExecutionContext),
                ["description"] = Description.Get(Context.ExecutionContext),
                ["manualdiscountamount"] = ManualDiscountAmount.Get(Context.ExecutionContext),
                ["tax"] = Tax.Get(Context.ExecutionContext)
            };

            var writeInProduct = ProductName.Get(Context.ExecutionContext);

            if (!string.IsNullOrEmpty(writeInProduct))
            {
                detailRecord["productdescription"] = writeInProduct;
                detailRecord["isproductoverridden"] = true;
                detailRecord["ispriceoverridden"] = true;
                detailRecord["priceperunit"] = PricePerUnit.Get(Context.ExecutionContext);
            }
            else
            {
                detailRecord["productid"] = ProductId.Get(Context.ExecutionContext);
                detailRecord["isproductoverridden"] = false;
                detailRecord["uomid"] = UomId.Get(Context.ExecutionContext);

                var ispriceoverridden = IsPriceOverridden.Get(Context.ExecutionContext);
                detailRecord["ispriceoverridden"] = ispriceoverridden;

                if (ispriceoverridden)
                {
                    detailRecord["priceperunit"] = PricePerUnit.Get(Context.ExecutionContext);
                }
            }

            ProcessAdditionalFields(ref detailRecord);

            Context.UserService.Create(detailRecord);
        }
    }
}
