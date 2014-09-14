using Newtonsoft.Json.Linq;

namespace ComPerWorkerRole
{
    public class JsonRpcReturnStatus
    {
        public JsonRpcError Error { get; set; }

        public JToken Result { get; set; }
    }
}
