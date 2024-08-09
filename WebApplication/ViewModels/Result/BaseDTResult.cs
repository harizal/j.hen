﻿using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace WApp.ViewModels.Result
{
    public class BaseDTResult<T>
    {
        [JsonProperty(PropertyName = "draw")]
        [JsonPropertyName("draw")]
        public int Draw { get; set; }

        [JsonProperty(PropertyName = "recordsTotal")]
        [JsonPropertyName("recordsTotal")]
        public int RecordsTotal { get; set; }

        [JsonProperty(PropertyName = "recordsFiltered")]
        [JsonPropertyName("recordsFiltered")]
        public int RecordsFiltered { get; set; }

        [JsonProperty(PropertyName = "data")]
        [JsonPropertyName("data")]
        public IEnumerable<T> Data { get; set; } = new List<T>();
    }
}
