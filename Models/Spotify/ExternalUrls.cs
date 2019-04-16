using Newtonsoft.Json;

namespace SpotifyPlayer.Models.Spotify
{
    public class ExternalUrls
    {
        [JsonProperty("spotify")]
        public string Spotify { get; set; }
    }
}