using System;
using Newtonsoft.Json;

namespace TaskTracker.Models
{
    public class Item
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "isComplete")]
        public bool Completed { get; set; }

        [JsonProperty(PropertyName = "completeDate")]
        public DateTime CompletedDate { get; set; }

        [JsonProperty(PropertyName = "user")]
        public string User { get; set; }

    }
}