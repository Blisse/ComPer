using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ComPerLibrary.Models;
using ComPerWindows.Services.Interfaces;
using JsonRpcModelsLibrary.Clients;
using Newtonsoft.Json.Linq;

namespace ComPerWindows.Services.Implementations
{
    public class PostSmallJsonRpcRequest : IJsonRpcRequest
    {
        public JToken GetContent()
        {
            var content = SmallObjectTestModel.Create();
            content.StartTime = DateTime.UtcNow.Ticks.ToString();
            return JToken.FromObject(content);
        }

        public string GetCommand()
        {
            return Command;
        }

        public const String Command = "Small";
    }

    public class PostLargeJsonRpcRequest : IJsonRpcRequest
    {
        public JToken GetContent()
        {
            var content = LargeObjectTestModel.Create();
            content.StartTime = DateTime.UtcNow.Ticks.ToString();
            return JToken.FromObject(content);
        }

        public string GetCommand()
        {
            return Command;
        }

        public const String Command = "Large";
    }

    public class TestService : ITestService
    {
        private readonly JsonRpcClient _jsonRpcClient;
        private readonly HttpClient _httpClient;

        private readonly String PostSmallUrl;
        private readonly String PostLargeUrl;

        public TestService()
        {
            if (true) // isOnline
            {
                PostSmallUrl = "http://comm-perf-test.cloudapp.net/AppComPerService.svc/json/PostSmall";
                PostLargeUrl = "http://comm-perf-test.cloudapp.net/AppComPerService.svc/json/PostLarge";
                _jsonRpcClient = new JsonRpcClient("23.99.17.197", "9000");
            }
            else
            {
                PostSmallUrl = "http://localhost:32627/AppComPerService.svc/json/PostSmall";
                PostLargeUrl = "http://localhost:32627/AppComPerService.svc/json/PostLarge";
                _jsonRpcClient = new JsonRpcClient("127.0.0.1", "9000");
            }


            _httpClient = new HttpClient();
        }

        public async Task Start()
        {
            await RestartAsync();
        }

        private async Task RestartAsync()
        {
            if (_jsonRpcClient.IsConnected)
            {
                _jsonRpcClient.Stop();
            }

            if (!_jsonRpcClient.IsConnected)
            {
                await _jsonRpcClient.ConnectAsync();
                _jsonRpcClient.Start();
            }
        }

        public async Task<String> PostRequestAsync(String postUrl, JToken postContent)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(postUrl);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";

            using (StreamWriter writer = new StreamWriter(await webRequest.GetRequestStreamAsync()))
            {
                await writer.WriteAsync(postContent.ToString());
            }

            HttpWebResponse response = (HttpWebResponse)await webRequest.GetResponseAsync();

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                var stringResponse = await reader.ReadToEndAsync();
                return stringResponse;
            }
        }

        public async Task<SmallObjectTestModel> PostSmallHttpAsync(CancellationToken token)
        {
            PostSmallJsonRpcRequest request = new PostSmallJsonRpcRequest();

            String responseString = await PostRequestAsync(PostSmallUrl, request.GetContent());
            SmallObjectTestModel testingModel = JToken.Parse(responseString).ToObject<SmallObjectTestModel>();
            testingModel.EndTime = DateTime.UtcNow.Ticks.ToString();
            return testingModel;
        }

        public async Task<LargeObjectTestModel> PostLargeHttpAsync(CancellationToken token)
        {
            PostLargeJsonRpcRequest request = new PostLargeJsonRpcRequest();

            String responseString = await PostRequestAsync(PostLargeUrl, request.GetContent());
            LargeObjectTestModel testingModel = JToken.Parse(responseString).ToObject<LargeObjectTestModel>();
            testingModel.EndTime = DateTime.UtcNow.Ticks.ToString();
            return testingModel;
        }

        public async Task<SmallObjectTestModel> PostSmallStreamAsync(CancellationToken token)
        {
            if (!_jsonRpcClient.IsConnected)
            {
                await RestartAsync();
            }

            if (_jsonRpcClient.IsConnected)
            {
                var response = await _jsonRpcClient.SendRequestAsync(new PostSmallJsonRpcRequest());
                if (!response.IsError)
                {
                    var small = response.Content.ToObject<SmallObjectTestModel>();
                    small.EndTime = DateTime.UtcNow.Ticks.ToString();

                    return small;
                }
            }

            return null;
        }

        public async Task<LargeObjectTestModel> PostLargeStreamAsync(CancellationToken token)
        {
            if (!_jsonRpcClient.IsConnected)
            {
                await RestartAsync();
            }

            if (_jsonRpcClient.IsConnected)
            {
                var response = await _jsonRpcClient.SendRequestAsync(new PostLargeJsonRpcRequest());
                if (!response.IsError)
                {
                    var large = response.Content.ToObject<LargeObjectTestModel>();
                    large.EndTime = DateTime.UtcNow.Ticks.ToString();

                    return large;
                }
            }

            return null;
        }
    }
}
