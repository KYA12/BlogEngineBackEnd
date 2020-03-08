using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace BackendTestTask.IntegrationTests
{
    /*Helper to serialize object to json*/
    public static class JsonSerializeHelper
    {
        public static StringContent GetStringContent(object obj)
            => new StringContent(JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), Encoding.Default, "application/json");
    }
}
