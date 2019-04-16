using Newtonsoft.Json;
using System;

namespace SpotifyPlayer.Models.Spotify
{
    public class AccessToken
    {
        [JsonProperty("access_token")]
        public string Token { get; private set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; private set; }
        [JsonProperty("expires_in")]
        public int Expiry { get; private set; }
        [JsonProperty("token_type")]
        public string TokenType { get; private set; }
        [JsonProperty("scope")]
        public string Scope { get; private set; }

        public DateTime ExpiryDate { get; set; }
    }
}
