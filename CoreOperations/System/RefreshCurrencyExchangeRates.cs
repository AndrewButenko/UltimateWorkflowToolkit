using System.Net;
using System.IO;
using System.Xml;
using System.Linq;
using System.Globalization;
using System.Activities;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.System
{

    #region Overriddes

    public class RefreshCurrencyExchangeRates : CrmWorkflowBase
    {
        protected override void ExecuteWorkflowLogic(CodeActivityContext executionContext, IWorkflowContext context, IOrganizationService service, IOrganizationService sysService)
        {
            #region Get Base Currency

            QueryExpression query = new QueryExpression("transactioncurrency")
            {
                ColumnSet = new ColumnSet("isocurrencycode")
            };
            query.AddLink("organization", "transactioncurrencyid", "basecurrencyid", JoinOperator.Inner);

            Entity BaseCurrency = service.RetrieveMultiple(query).Entities.FirstOrDefault();

            if (BaseCurrency == null)
                return;

            var BaseCurrencyCode = BaseCurrency.GetAttributeValue<string>("isocurrencycode").ToUpper();
            var BaseCurrencyId = BaseCurrency.Id;

            #endregion Get Base Currency

            #region Getting All Currencies Except Base Currency

            query = new QueryExpression("transactioncurrency")
            {
                ColumnSet = new ColumnSet("isocurrencycode")
            };
            query.Criteria.AddCondition("transactioncurrencyid", ConditionOperator.NotEqual, BaseCurrencyId);

            List<Entity> allCurrencies = service.RetrieveMultiple(query).Entities.ToList();

            #endregion Getting All Currencies Except Base Currency

            #region Looping through currencies and updating Exhange Rates

            foreach (Entity currency in allCurrencies)
            {
                string currencyCode = currency.GetAttributeValue<string>("isocurrencycode").ToUpper();

                string url = "http://www.webservicex.net/CurrencyConvertor.asmx/ConversionRate?FromCurrency=" + BaseCurrencyCode + "&ToCurrency=" + currencyCode;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                using (StreamReader resStream = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    var doc = new XmlDocument();
                    string xmlResult = resStream.ReadToEnd();
                    doc.LoadXml(xmlResult);

                    decimal rate = decimal.Parse(doc.DocumentElement.FirstChild.Value, CultureInfo.InvariantCulture);

                    currency.Attributes.Clear();
                    currency["exchangerate"] = rate;

                    service.Update(currency);
                }
            }

            #endregion Looping through currencies and updating Exhange Rates
        }
    }

    #endregion Overriddes
}
