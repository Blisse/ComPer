using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ComPerLibrary.Models;

namespace ComPerWebRole
{
    [ServiceContract]
    public interface IAppComPerService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/json/PostSmall", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        SmallObjectTestModel PostSmallObject(SmallObjectTestModel simpleJsonModel);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/json/PostLarge", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        LargeObjectTestModel PostLargeObject(LargeObjectTestModel simpleJsonModel);
    }
}
