using SpotifyPlayer.Models.Spotify;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SpotifyPlayer.Services.Spotify.Player
{
    public class PlayerService : BaseHttpClient
    {
        private const string _endpoint = "https://api.spotify.com/v1/me/player";
        private const string _authenticationSchema = "Bearer";
        public PlayerService()
        {
            base.BaseUrl = _endpoint;
        }
        internal async Task Play(AccessToken accessToken)
        {
            var endpoint = "/play";
            await base.Put(endpoint, new AuthenticationHeaderValue(_authenticationSchema, accessToken.Token));
        }
        internal async Task Pause(AccessToken accessToken)
        {
            var endpoint = "/pause";
            await base.Put(endpoint, new AuthenticationHeaderValue(_authenticationSchema, accessToken.Token));
        }
        internal async Task<bool> Next(AccessToken accessToken)
        {
            var endpoint = "/next";
            return await base.Post(endpoint, new AuthenticationHeaderValue(_authenticationSchema, accessToken.Token));
        }
        internal async Task<bool> Previous(AccessToken accessToken)
        {
            var endpoint = "/previous";
            return await base.Post(endpoint, new AuthenticationHeaderValue(_authenticationSchema, accessToken.Token));
        }
        internal async Task<bool> PlayTrack(AccessToken accessToken, string context)
        {
            var endpoint = "/play";
            return await base.Put(endpoint, new AuthenticationHeaderValue(_authenticationSchema, accessToken.Token), context);
        }
        internal async Task Shuffle(AccessToken accessToken, dynamic ctx)
        {
            var endpoint = $"/shuffle?state={ctx}";
            await base.Put(endpoint, new AuthenticationHeaderValue(_authenticationSchema, accessToken.Token));
        }
        internal async Task Repeat(AccessToken accessToken, dynamic ctx)
        {
            var endpoint = $"/repeat?state={ctx}";
            await base.Put(endpoint, new AuthenticationHeaderValue(_authenticationSchema, accessToken.Token));
        }
    }
}
