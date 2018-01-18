using System.Net;
using System.IO;
using System.Linq;
using System.Activities;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.System
{


    public class RefreshCurrencyExchangeRates : CrmWorkflowBase
    {

        #region Overriddes

        protected override void ExecuteWorkflowLogic()
        {
            #region Get All Currencies From Endpoint and check that call was successfull

            string jsonResult = null;

            var url = "http://apilayer.net/api/live?access_key=" + Context.Settings.CurrencylayerKey;
            var request = (HttpWebRequest)WebRequest.Create(url);
            using (var resStream = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                jsonResult = resStream.ReadToEnd();
            }

            var jobject = JObject.Parse(jsonResult);

            var success = jobject.SelectToken("$.success").Value<bool>();

            if (!success)
            {
                var errorToken = jobject.SelectToken("$.error");
                var errorMessage = $@"Can't obtain currency exchange rates:
Code: {errorToken.SelectToken("code").Value<int>()}
Type: {errorToken.SelectToken("type").Value<string>()}
Info: {errorToken.SelectToken("info").Value<string>()}";

                throw new InvalidPluginExecutionException(errorMessage);
            }

            #endregion Get All Currencies From Endpoint and check that call was successfull

            #region Get Base Currency

            QueryExpression query = new QueryExpression("transactioncurrency")
            {
                ColumnSet = new ColumnSet("isocurrencycode", "currencyname")
            };
            query.AddLink("organization", "transactioncurrencyid", "basecurrencyid", JoinOperator.Inner);

            var baseCurrency = Context.SystemService.RetrieveMultiple(query).Entities.FirstOrDefault();

            if (baseCurrency == null)
                return;

            var baseCurrencyCode = baseCurrency.GetAttributeValue<string>("isocurrencycode").ToUpper();
            var baseCurrencyId = baseCurrency.Id;
            var baseCurrencyName = baseCurrency.GetAttributeValue<string>("currencyname");

            var baseCurrencyNode = jobject.SelectToken($"$.quotes.USD{baseCurrencyCode}");

            if (baseCurrencyNode == null)
            {
                throw new InvalidPluginExecutionException($"Exchange Rates for your Base Currency ({baseCurrencyName}) are not available");
            }

            var usdToBaseCurrencyRate = baseCurrencyNode.Value<decimal>();

            #endregion Get Base Currency

            #region Getting All Currencies Except Base Currency

            query = new QueryExpression("transactioncurrency")
            {
                ColumnSet = new ColumnSet("isocurrencycode", "currencyname")
            };
            query.Criteria.AddCondition("transactioncurrencyid", ConditionOperator.NotEqual, baseCurrencyId);

            List<Entity> allCurrencies = Context.SystemService.RetrieveMultiple(query).Entities.ToList();

            #endregion Getting All Currencies Except Base Currency

            #region Looping through currencies and updating Exhange Rates

            foreach (Entity currency in allCurrencies)
            {
                var currencyCode = currency.GetAttributeValue<string>("isocurrencycode").ToUpper();
                var currencyName = currency.GetAttributeValue<string>("currencyname");

                var currencyNode = jobject.SelectToken($"$.quotes.USD{currencyCode}");

                if (currencyNode == null)
                {
                    Context.TracingService.Trace($"Can't refresh exchange rate for {currencyName} currency");
                    continue;
                }

                var usdToCurrencyRate = currencyNode.Value<decimal>();


                decimal rate = usdToCurrencyRate / usdToBaseCurrencyRate;

                currency.Attributes.Clear();
                currency["exchangerate"] = rate;

                Context.SystemService.Update(currency);
            }

            #endregion Looping through currencies and updating Exhange Rates
        }
    }

    #endregion Overriddes
}
