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
using Newtonsoft.Json.Linq;

namespace UltimateWorkflowToolkit.CoreOperations.System
{


    public class RefreshCurrencyExchangeRates : CrmWorkflowBase
    {
        #region Input/Output Arguments

        [Input("Currency Layer Access Key")]
        [RequiredArgument]
        public InArgument<string> AcccessKey { get; set; }

        #endregion Input/Output Arguments

        #region Overriddes

        protected override void ExecuteWorkflowLogic()
        {
            #region Get All Currencies From Endpoint and check that call was successfull

            string jsonResult = null;

            string url = "http://apilayer.net/api/live?access_key=" + AcccessKey.Get(Context.ExecutionContext);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            using (StreamReader resStream = new StreamReader(request.GetResponse().GetResponseStream()))
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

            Entity BaseCurrency = Context.SystemService.RetrieveMultiple(query).Entities.FirstOrDefault();

            if (BaseCurrency == null)
                return;

            var BaseCurrencyCode = BaseCurrency.GetAttributeValue<string>("isocurrencycode").ToUpper();
            var BaseCurrencyId = BaseCurrency.Id;
            var BaseCurrencyName = BaseCurrency.GetAttributeValue<string>("currencyname");

            var BaseCurrencyNode = jobject.SelectToken($"$.quotes.USD{BaseCurrencyCode}");

            if (BaseCurrencyNode == null)
            {
                throw new InvalidPluginExecutionException($"Exchange Rates for your Base Currency ({BaseCurrencyName}) are not available");
            }

            var usdToBaseCurrencyRate = BaseCurrencyNode.Value<decimal>();

            #endregion Get Base Currency

            #region Getting All Currencies Except Base Currency

            query = new QueryExpression("transactioncurrency")
            {
                ColumnSet = new ColumnSet("isocurrencycode", "currencyname")
            };
            query.Criteria.AddCondition("transactioncurrencyid", ConditionOperator.NotEqual, BaseCurrencyId);

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
