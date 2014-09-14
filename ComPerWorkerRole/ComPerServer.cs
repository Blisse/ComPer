using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using ComPerLibrary.Models;
using Newtonsoft.Json.Linq;

namespace ComPerWorkerRole
{
    public class ComPerServer
    {
        private readonly JsonRpcStreamClient _jsonRpcStreamClient;
        private readonly ConcurrentDictionary<String, TaskCompletionSource<JsonRpcResponse>> _pendingRequests;

        public ComPerServer(NetworkStream networkStream)
        {
            _jsonRpcStreamClient = new JsonRpcStreamClient(new StreamReader(networkStream), new StreamWriter(networkStream));
            _pendingRequests = new ConcurrentDictionary<String, TaskCompletionSource<JsonRpcResponse>>();

            _jsonRpcStreamClient.RequestReceivedHandler += OnReceivedRequestHandler;
            _jsonRpcStreamClient.ResponseReceivedHandler += OnReceivedResponseHandler;
        }

        public ComPerServer(TcpClient tcpClient) : this(tcpClient.GetStream())
        {
        }

        public void Start()
        {
            _jsonRpcStreamClient.Start();
        }


        private void OnReceivedResponseHandler(object sender, JsonRpcResponse jsonRpcResponse)
        {
            if (jsonRpcResponse != null)
            {
                var responseId = jsonRpcResponse.Id;
                TaskCompletionSource<JsonRpcResponse> requestTcs;
                if (_pendingRequests.TryRemove(responseId, out requestTcs))
                {
                    requestTcs.SetResult(jsonRpcResponse);
                }
                else
                {
                    Debug.WriteLine("Received unmatched JsonRpcResponse.");
                }
            }
        }

        private void OnReceivedRequestHandler(object sender, JsonRpcRequest jsonRpcRequest)
        {
            JsonRpcResponse jsonRpcResponse;
            switch (jsonRpcRequest.Method)
            {
                case "Large":
                    jsonRpcResponse = OnReceivedLarge(jsonRpcRequest);
                    break;
                case "Small":
                    jsonRpcResponse = OnReceivedSmall(jsonRpcRequest);
                    break;
                default:
                    throw new NotSupportedException();
            }

            if (!jsonRpcRequest.IsNotification)
            {
                _jsonRpcStreamClient.SendResponseToClient(jsonRpcResponse);
            }
        }

        private JsonRpcResponse OnReceivedLarge(JsonRpcRequest jsonRpcRequest)
        {
            var testModel = jsonRpcRequest.Params.ToObject<LargeObjectTestModel>();
            testModel.ServerReceivedTime = DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture);

            return JsonRpcResponse.Factory.CreateSuccessfulJsonRpcResponse(jsonRpcRequest, JToken.FromObject(testModel));
        }

        private JsonRpcResponse OnReceivedSmall(JsonRpcRequest jsonRpcRequest)
        {
            var testModel = jsonRpcRequest.Params.ToObject<SmallObjectTestModel>();
            testModel.ServerReceivedTime = DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture);

            return JsonRpcResponse.Factory.CreateSuccessfulJsonRpcResponse(jsonRpcRequest, JToken.FromObject(testModel));
        }

    }
}
