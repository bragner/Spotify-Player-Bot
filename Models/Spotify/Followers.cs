using Newtonsoft.Json;

namespace SpotifyPlayer.Models.Spotify
{
    public class Followers
    {
        [JsonProperty("href")]
        public object Href { get; set; }
        [JsonProperty("total")]
        public int Total { get; set; }
    }
}
