using Newtonsoft.Json;

namespace SpotifyPlayer.Models.Spotify
{
    public class SearchResult
    {
        [JsonProperty("artists")]
        public ArtistsResult Artists { get; set; }
        [JsonProperty("tracks")]
        public TracksResult Tracks { get; set; }
    }
}
