using System.Text.Json.Serialization;

namespace TestProject1
{
    internal class Activity
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("completed")]
        public bool Completed { get; set; }
    }
}
