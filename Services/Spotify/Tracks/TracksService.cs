using SpotifyPlayer.Models.Spotify;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SpotifyPlayer.Services.Spotify.Tracks
{
    public class TracksService : BaseHttpClient
    {
        private const string _endpoint = "https://api.spotify.com/v1";
        private const string _authenticationSchema = "Bearer";

        public TracksService()
        {
            base.BaseUrl = _endpoint;
        }

        internal async Task<string> GetTrack(AccessToken accessToken, string trackId)
        {
            var endpoint = $"/tracks/{trackId}/";
            return await base.Get(endpoint, new AuthenticationHeaderValue(_authenticationSchema, accessToken.Token));
        }
    }
}
