using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace KHueHelper
{
    public class JsonClasses
    {
        public class HueBridgeInfo
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("internalipaddress")]
            public string InternalIPAddress { get; set; }

        }


        public class HueBridgeAuthError
        {
            [JsonPropertyName("type")]
            public int Type { get; set; }

            [JsonPropertyName("address")]
            public string Address { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }
        }

        public class HueBridgeAuthRoot
        {
            [JsonPropertyName("error")]
            public HueBridgeAuthError Error { get; set; }
            [JsonPropertyName("success")]
            public HueBridgeAuthSuccess Success { get; set; }
        }

        public class HueBridgeAuthSuccess
        {
            [JsonPropertyName("username")]
            public string Username { get; set; }
        }


    }
}
