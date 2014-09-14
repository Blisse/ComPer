using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ComPerLibrary.Models
{
    [DataContract]
    [JsonObject]
    public class TestingModel
    {
        [DataMember]
        [JsonProperty]
        public String StartTime { get; set; }

        [DataMember]
        [JsonProperty]
        public String ServerReceivedTime { get; set; }

        [DataMember]
        [JsonProperty]
        public String EndTime { get; set; }
    }

    [DataContract]
    [JsonObject]
    public class LargeObjectTestModel : TestingModel
    {
        [DataMember]
        [JsonProperty]
        public String Username { get; set; }

        [DataMember]
        [JsonProperty]
        public String Name { get; set; }

        [DataMember]
        [JsonProperty]
        public String BirthDate { get; set; }

        [DataMember]
        [JsonProperty]
        public String BirthPlace { get; set; }

        [DataMember]
        [JsonProperty]
        public String CurrentLocation { get; set; }

        [DataMember]
        [JsonProperty]
        public String Notes { get; set; }

        public static LargeObjectTestModel Create()
        {
            var model = new LargeObjectTestModel()
            {
                BirthDate = DateTime.UtcNow.AddYears(-20).ToString(),
                BirthPlace = "Hospital",
                CurrentLocation = "Hospital",
                Name = "Giant Hospital",
                Notes = "Don't fall for the hospital",
                Username = "Giant_Hospital"
            };

            return model;
        }
    }

    [DataContract]
    [JsonObject]
    public class SmallObjectTestModel : TestingModel
    {
        [DataMember]
        [JsonProperty]
        public String Title { get; set; }

        public static SmallObjectTestModel Create()
        {
            var model = new SmallObjectTestModel()
            {
                Title = "Lying Treehouse"
            };

            return model;
        }
    }
}
