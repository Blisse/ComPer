﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ComPerWorkerRole.Utilities
{
    public static class JTokenExtensions
    {
        public static bool IsJsonRpcErrorObject(this JToken jToken)
        {
            if (jToken is JArray)
            {
                // Errors are a single object
                return false;
            }

            if (jToken is JObject)
            {
                return jToken["code"] != null && jToken["message"] != null;
            }

            return false;
        }
    }
}
