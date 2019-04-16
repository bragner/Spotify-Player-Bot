using Newtonsoft.Json;

namespace SpotifyPlayer.Models.Spotify
{
    public class CurrentTrack
    {
        [JsonProperty("context")]
        public Context Context { get; set; }
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
        [JsonProperty("progress_ms")]
        public int ProgressMs { get; set; }
        [JsonProperty("is_playing")]
        public bool IsPlaying { get; set; }
        [JsonProperty("currently_playing_type")]
        public string CurrentlyPlayingType { get; set; }
        [JsonProperty("item")]
        public Track Track { get; set; }
    }
}
