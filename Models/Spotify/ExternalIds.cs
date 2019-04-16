using Newtonsoft.Json;

namespace SpotifyPlayer.Models.Spotify
{
    public class ExternalIds
    {
        [JsonProperty("isrc")]
        public string Isrc { get; set; }
    }
}