using SpotifyPlayer.Models.Spotify;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SpotifyPlayer.Services.Spotify.Artists
{
    public class ArtistsService : BaseHttpClient
    {
        private const string _endpoint = "https://api.spotify.com/v1";
        private const string _authenticationSchema = "Bearer";

        public ArtistsService()
        {
            base.BaseUrl = _endpoint;
        }

        internal async Task<string> GetArtistTopTracks(AccessToken accessToken, string artistId)
        {
            var endpoint = $"/artists/{artistId}/top-tracks?country=from_token";
            return await base.Get(endpoint, new AuthenticationHeaderValue(_authenticationSchema, accessToken.Token));
        }
    }
}
