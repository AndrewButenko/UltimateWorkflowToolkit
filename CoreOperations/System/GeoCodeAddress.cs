using System;
using System.Activities;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Newtonsoft.Json;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.System
{
    public class GeoCodeAddress: CrmWorkflowBase
    {
        #region Inputs

        [Input("Address")]
        [RequiredArgument]
        public InArgument<string> Address { get; set; }

        [Output("Success")]
        public OutArgument<bool> IsResolved { get; set; }

        [Output("Longitude")]
        public OutArgument<decimal> Longitude { get; set; }

        [Output("Latitude")]
        public  OutArgument<decimal> Latitude { get; set; }

        #endregion Inputs

        protected override void ExecuteWorkflowLogic()
        {
            if (string.IsNullOrWhiteSpace(Context.Settings.BingMapsKey))
                throw new InvalidPluginExecutionException("BingMaps Key setting is not available.");

            var address = Address.Get(Context.ExecutionContext);

            if (string.IsNullOrWhiteSpace(address))
                throw new InvalidPluginExecutionException("Address parameter is not available");

            var url = $"http://dev.virtualearth.net/REST/v1/Locations?q={address}&key={Context.Settings.BingMapsKey}";

            string jsonResult = null;

            var request = (HttpWebRequest)WebRequest.Create(url);
            using (var resStream = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                jsonResult = resStream.ReadToEnd();
            }

            var response = JsonConvert.DeserializeObject<Response>(jsonResult);

            if (response.StatusCode != 200)
            {
                throw new InvalidPluginExecutionException($"BingMaps Endpoint call failed - {string.Join(Environment.NewLine, response.ErrorDetails)}{Environment.NewLine}{response.StatusDescription}");
            }

            IsResolved.Set(Context.ExecutionContext, true);
            Latitude.Set(Context.ExecutionContext, Convert.ToDecimal(response.ResourceSets[0].Resources[0].GeocodePoints[0].Coordinates[0]));
            Longitude.Set(Context.ExecutionContext, Convert.ToDecimal(response.ResourceSets[0].Resources[0].GeocodePoints[0].Coordinates[1]));
        }
    }

    #region Contract Classes

    public class Response
    {
        [JsonProperty("copyright")]
        public string Copyright { get; set; }

        [JsonProperty("brandLogoUri")]
        public string BrandLogoUri { get; set; }

        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("statusDescription")]
        public string StatusDescription { get; set; }

        [JsonProperty("authenticationResultCode")]
        public string AuthenticationResultCode { get; set; }

        [JsonProperty("errorDetails")]
        public string[] ErrorDetails { get; set; }

        [JsonProperty("traceId")]
        public string TraceId { get; set; }

        [JsonProperty("resourceSets")]
        public ResourceSet[] ResourceSets { get; set; }
    }

    public class ResourceSet
    {
        [JsonProperty("estimatedTotal")]
        public long EstimatedTotal { get; set; }

        [JsonProperty("resources")]
        public Location[] Resources { get; set; }
    }

    public class Point
    {
        /// <summary>
        /// Latitude,Longitude
        /// </summary>
        [JsonProperty("coordinates")]
        public double[] Coordinates { get; set; }
    }

    public class BoundingBox
    {
        [JsonProperty("southLatitude")]
        public double SouthLatitude { get; set; }

        [JsonProperty("westLongitude")]
        public double WestLongitude { get; set; }

        [JsonProperty("northLatitude")]
        public double NorthLatitude { get; set; }

        [JsonProperty("eastLongitude")]
        public double EastLongitude { get; set; }
    }

    public class GeocodePoint : Point
    {
        [JsonProperty("calculationMethod")]
        public string CalculationMethod { get; set; }

        [JsonProperty("usageTypes")]
        public string[] UsageTypes { get; set; }
    }

    public class Location
    {
        [JsonProperty("boundingBox")]
        public BoundingBox BoundingBox { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("point")]
        public Point Point { get; set; }

        [JsonProperty("entityType")]
        public string EntityType { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }

        [JsonProperty("confidence")]
        public string Confidence { get; set; }

        [JsonProperty("geocodePoints")]
        public GeocodePoint[] GeocodePoints { get; set; }

        [JsonProperty("matchCodes")]
        public string[] MatchCodes { get; set; }
    }

    public class Address
    {
        [JsonProperty("addressLine")]
        public string AddressLine { get; set; }

        [JsonProperty("adminDistrict")]
        public string AdminDistrict { get; set; }

        [JsonProperty("adminDistrict2")]
        public string AdminDistrict2 { get; set; }

        [JsonProperty("countryRegion")]
        public string CountryRegion { get; set; }

        [JsonProperty("formattedAddress")]
        public string FormattedAddress { get; set; }

        [JsonProperty("locality")]
        public string Locality { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }
    }

    #endregion Contract Classes

}
