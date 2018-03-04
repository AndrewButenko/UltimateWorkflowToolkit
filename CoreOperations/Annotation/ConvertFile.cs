using System;
using System.IO;
using System.Net;
using System.Text;
using System.Activities;
using Newtonsoft.Json;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace UltimateWorkflowToolkit.CoreOperations.Annotation
{
    public class ConvertFile: OperationBaseFile
    {
        #region Inputs/Outputs

        [Input("Replace Original File")]
        [RequiredArgument]
        public InArgument<bool> IsReplaceOriginalFile { get; set; }

        #endregion Inputs/Outputs

        protected override void ExecuteOperation(Entity file, EntityReference parentRecord, string currentFileName, string resultingFileName)
        {
            var resultingFileNameParts = resultingFileName.Split('.');
            var currentFileNameParts = currentFileName.Split('.');

            var baseUrl = "https://api.cloudconvert.com/convert";

            var request = WebRequest.Create(baseUrl);

            request.Method = "POST";
            request.ContentType = "application/json";

            var convertFileRequest = new ConvertFileRequest()
            {
                ApiKey = Context.Settings.CloudConvertKey,
                File = file.GetAttributeValue<string>("documentbody"),
                OutputFormat = resultingFileNameParts[resultingFileNameParts.Length - 1],
                InputMethod = "base64",
                FileName = currentFileName,
                InputFormat = currentFileNameParts[currentFileNameParts.Length - 1]
            };

            var requestBodyString = JsonConvert.SerializeObject(convertFileRequest);
            var requestData = Encoding.Default.GetBytes(requestBodyString);

            using (var stream = request.GetRequestStream())
            {
                stream.Write(requestData, 0, requestData.Length);
            }

            var inputStream = request.GetResponse().GetResponseStream();

            var memoStream = new MemoryStream();
            inputStream.CopyTo(memoStream);
            inputStream.Close();

            string newFileContent;

            memoStream.Position = 0;

            using (var msr = new StreamReader(memoStream, Encoding.Default))
            {
                newFileContent = msr.ReadToEnd();
            }

            memoStream.Close();

            var attachment = new Entity("annotation")
            {
                ["subject"] = file.GetAttributeValue<string>("subject"),
                ["filename"] = resultingFileName,
                ["documentbody"] = Convert.ToBase64String(Encoding.Default.GetBytes(newFileContent)),
                ["isdocument"] = true,
                ["objectid"] = parentRecord
            };

            var isReplaceOriginal = IsReplaceOriginalFile.Get(Context.ExecutionContext);

            if (isReplaceOriginal)
            {
                attachment.Id = file.Id;
                Context.UserService.Update(attachment);
            }
            else
            {
                Context.UserService.Create(attachment);
            }
        }
    }

    public class ConvertFileRequest
    {
        [JsonProperty("apikey")]
        public string ApiKey { get; set; }

        [JsonProperty("inputformat")]
        public string InputFormat { get; set; }

        [JsonProperty("outputformat")]
        public string OutputFormat { get; set; }

        [JsonProperty("input")]
        public string InputMethod { get; set; }

        [JsonProperty("file")]
        public string File { get; set; }

        [JsonProperty("filename")]
        public string FileName { get; set; }

        [JsonProperty("wait")]
        public bool Wait => true;

        [JsonProperty("download")]
        public bool Download => true;
    }
}
