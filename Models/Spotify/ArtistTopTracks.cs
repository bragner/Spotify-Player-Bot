using Newtonsoft.Json;
using System.Collections.Generic;

namespace SpotifyPlayer.Models.Spotify
{
    public class ArtistTopTracks
    {
        [JsonProperty("tracks")]
        public List<Track> Tracks { get; set; }
    }
}
