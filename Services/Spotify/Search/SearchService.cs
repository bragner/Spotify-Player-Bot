using SpotifyPlayer.Models.Spotify;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SpotifyPlayer.Services.Spotify.Search
{
    public class SearchService : BaseHttpClient
    {
        private const string _endpoint = "https://api.spotify.com/v1";
        private const string _authenticationSchema = "Bearer";
        public SearchService()
        {
            this.BaseUrl = _endpoint;
        }
        internal async Task<string> Search(AccessToken accessToken, string query)
        {
            var endpoint = $"/search?q={query.Replace(" ", "%20")}&type=artist,track";
            return await base.Get(endpoint, new AuthenticationHeaderValue(_authenticationSchema, accessToken.Token));
        }
        internal async Task<string> SearchTrack(AccessToken accessToken, string query)
        {
            var endpoint = $"/search?q={query.Replace(" ", "%20")}&type=track";
            return await base.Get(endpoint, new AuthenticationHeaderValue(_authenticationSchema, accessToken.Token));
        }
        internal async Task<string> SearchArtist(AccessToken accessToken, string query)
        {
            var endpoint = $"/search?q={query.Replace(" ", "%20")}&type=artist";
            return await base.Get(endpoint, new AuthenticationHeaderValue(_authenticationSchema, accessToken.Token));
        }
    }
}
