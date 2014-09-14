using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ComPerLibrary.Models
{
    public interface IJsonReader
    {
        Task<JObject> ReadJObjectAsync();
    }
}
