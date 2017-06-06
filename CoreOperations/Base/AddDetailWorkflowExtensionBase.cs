using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.CoreOperations.Base
{
    public abstract class AddDetailWorkflowExtensionBase: AddDetailWorkflowBase
    {
        #region Inputs

        [Input("Salesperson")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> SalesRepId { get; set; }

        [Input("Ship To - Address(False) or Will Call(True)")]
        public InArgument<bool> WillCall { get; set; }

        [Input("Ship To Name")]
        public InArgument<string> ShipToName { get; set; }

        [Input("Ship To Street 1")]
        public InArgument<string> ShipToLine1 { get; set; }

        [Input("Ship To Street 2")]
        public InArgument<string> ShipToLine2 { get; set; }

        [Input("Ship To Street 3")]
        public InArgument<string> ShipToLine3 { get; set; }

        [Input("Ship To City")]
        public InArgument<string> ShipToCity { get; set; }

        [Input("Ship To State/Province")]
        public InArgument<string> ShipToStateOrProvince { get; set; }

        [Input("Ship To Zip/Postal Code")]
        public InArgument<string> ShipToPostalCode { get; set; }

        [Input("Ship To Contact Name")]
        public InArgument<string> ShipToContactName { get; set; }

        [Input("Ship To Country/Region")]
        public InArgument<string> ShipToCountry { get; set; }

        [Input("Ship To Phone")]
        public InArgument<string> ShipToTelephone { get; set; }

        [Input("Ship To Fax")]
        public InArgument<string> ShipToFax { get; set; }

        #endregion Inputs

        #region Overrides

        protected override void ProcessAdditionalFields(ref Entity record, CodeActivityContext executionContext)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));

            record["salesrepid"] = SalesRepId.Get(executionContext);
            record["shipto_city"] = ShipToCity.Get(executionContext);
            record["shipto_contactname"] = ShipToContactName.Get(executionContext);
            record["shipto_country"] = ShipToCountry.Get(executionContext);
            record["shipto_fax"] = ShipToFax.Get(executionContext);
            record["shipto_line1"] = ShipToLine1.Get(executionContext);
            record["shipto_line2"] = ShipToLine2.Get(executionContext);
            record["shipto_line3"] = ShipToLine3.Get(executionContext);
            record["shipto_name"] = ShipToName.Get(executionContext);
            record["shipto_postalcode"] = ShipToPostalCode.Get(executionContext);
            record["shipto_stateorprovince"] = ShipToStateOrProvince.Get(executionContext);
            record["shipto_telephone"] = ShipToTelephone.Get(executionContext);
            record["willcall"] = WillCall.Get(executionContext);
        }

        #endregion Overrides

    }
}
