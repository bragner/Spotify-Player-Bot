using Newtonsoft.Json;
using System.Collections.Generic;

namespace SpotifyPlayer.Models.Spotify
{
    public class ArtistsResult
    {
        [JsonProperty("href")]
        public string Href { get; set; }
        [JsonProperty("items")]
        public List<Artist> Artists { get; set; }
        [JsonProperty("limit")]
        public int Limit { get; set; }
        [JsonProperty("next")]
        public string Next { get; set; }
        [JsonProperty("offset")]
        public int Offset { get; set; }
        [JsonProperty("previous")]
        public object Previous { get; set; }
        [JsonProperty("total")]
        public int Total { get; set; }
    }
}