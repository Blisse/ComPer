using System;
using System.Runtime.Serialization;
using ComPerWorkerRole;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ComPerLibrary.Models
{
    [JsonObject]
    public class JsonRpcResponse
    {
        [JsonProperty("jsonrpc", Required = Required.Always)]
        public String JsonRpc { get; set; }

        [JsonProperty("error", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public JsonRpcError Error { get; set; }

        [JsonProperty("result", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public JToken Result { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public String Id { get; set; }

        public JsonRpcResponse()
        {
            JsonRpc = JsonRpcConstants.Version;
        }

        protected JsonRpcResponse(JsonRpcRequest jsonRpcRequest)
        {
            if (jsonRpcRequest.IsNotification)
            {
                throw new NotSupportedException("Cannot respond to a notification.");
            }

            JsonRpc = JsonRpcConstants.Version;
            Id = jsonRpcRequest.Id;
        }

        public static class Factory
        {
            public static JsonRpcResponse CreateSuccessfulJsonRpcResponse(JsonRpcRequest request, JToken result)
            {
                return new JsonRpcResponse(request)
                {
                    Result = result
                };
            }

            public static JsonRpcResponse CreateErrorJsonRpcResponse(JsonRpcRequest request, String errorMessage)
            {
                return new JsonRpcResponse(request)
                {
                    Error = new JsonRpcError()
                    {
                        Code = 0,
                        Message = errorMessage
                    }
                };
            }
        }
    }
}
