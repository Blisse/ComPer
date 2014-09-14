using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ComPerLibrary.Models;
using Newtonsoft.Json.Linq;

namespace ComPerWebRole
{
    public class AppComPerService : IAppComPerService
    {
        public SmallObjectTestModel PostSmallObject(SmallObjectTestModel simpleJsonModel)
        {
            var content = simpleJsonModel;
            content.ServerReceivedTime = DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture);
            return content;
        }

        public LargeObjectTestModel PostLargeObject(LargeObjectTestModel simpleJsonModel)
        {
            var content = simpleJsonModel;
            content.ServerReceivedTime = DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture);
            return content;
        }
    }
}
