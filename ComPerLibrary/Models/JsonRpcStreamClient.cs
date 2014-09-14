using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ComPerWorkerRole;
using ComPerWorkerRole.Utilities;
using Newtonsoft.Json.Linq;

namespace ComPerLibrary.Models
{
    public class JsonRpcStreamClient
    {
        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (value == _isConnected)
                {
                    return;
                }

                _isConnected = value;

                if (ConnectionStatusChanged != null)
                {
                    ConnectionStatusChanged(this, _isConnected);
                }
            }
        }

        private readonly StreamReader _streamReader;

        private readonly StreamWriter _streamWriter;

        private readonly ConcurrentQueue<JObject> _sendToClientQueue;

        public EventHandler<JsonRpcRequest> RequestReceivedHandler { get; set; }
        public EventHandler<JsonRpcResponse> ResponseReceivedHandler { get; set; }
        public EventHandler<bool> ConnectionStatusChanged { get; set; }

        public JsonRpcStreamClient(StreamReader streamReader, StreamWriter streamWriter)
        {
            _streamReader = streamReader;
            _streamWriter = streamWriter;

            _sendToClientQueue = new ConcurrentQueue<JObject>();

            IsConnected = streamReader.BaseStream.CanRead && streamWriter.BaseStream.CanWrite;
        }

        public JsonRpcStreamClient(Stream inputStream, Stream outputStream)
            : this(new StreamReader(inputStream), new StreamWriter(outputStream))
        {
            
        }

        public void Start()
        {
            var receiveMessagesTask = Task.Run(() => ReceiveMessagesFromClientAsync());
            var sendMessagesTask = Task.Run(() => SendQueuedMessagesToClientAsync());
        }

        public void Stop()
        {
            IsConnected = false;
        }

        private async Task ReceiveMessagesFromClientAsync()
        {
            try
            {
                var jsonReader = new JsonBracketReader(_streamReader.BaseStream);

                while (IsConnected)
                {
                    var jObject = await jsonReader.ReadJObjectAsync();

                    Debug.WriteLine("ReadJObject: {0}", jObject);

                    if (jObject != null)
                    {
                        if (jObject.IsJsonRpcRequest())
                        {
                            var jsonRpcRequest = jObject.ToObject<JsonRpcRequest>();
                            
                            if (jsonRpcRequest != null)
                            {
                                if (RequestReceivedHandler != null)
                                {
                                    RequestReceivedHandler(this, jsonRpcRequest);
                                }
                            }

                        }
                        else if (jObject.IsJsonRpcResponse())
                        {
                            var jsonRpcResponse = jObject.ToObject<JsonRpcResponse>();

                            if (jsonRpcResponse != null)
                            {
                                if (ResponseReceivedHandler != null)
                                {
                                    ResponseReceivedHandler(this, jsonRpcResponse);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Caught exception: {0}", e);
                IsConnected = false;
            }
        }

        private async Task SendQueuedMessagesToClientAsync()
        {
            try
            {
                while (IsConnected)
                {
                    JObject jsonObject;
                    if (_sendToClientQueue.TryDequeue(out jsonObject))
                    {
                        await _streamWriter.WriteAsync(jsonObject.ToString());
                        await _streamWriter.FlushAsync();
                    }
                    else
                    {
                        await Task.Delay(200);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Caught exception: {0}", e);
                IsConnected = false;
            }
        }

        public void SendRequestToClient(JsonRpcRequest request)
        {
            AddJObjectToQueueAsync(JObject.FromObject(request));
        }

        public void SendResponseToClient(JsonRpcResponse response)
        {
            AddJObjectToQueueAsync(JObject.FromObject(response));
        }

        private void AddJObjectToQueueAsync(JObject jObject)
        {
            _sendToClientQueue.Enqueue(jObject);
        }
    }
}
