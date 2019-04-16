using Microsoft.Bot.Builder;
using Newtonsoft.Json;
using SpotifyPlayer.Models.Spotify;
using SpotifyPlayer.Services.Spotify.OAuth;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SpotifyPlayer.Services.Spotify.Player.CurrentTrackService
{

    public class CurrentTrackService : BaseHttpClient
    {
        private const string _endpoint = "https://api.spotify.com/v1/me/player";
        private const string _authenticationSchema = "Bearer";

        public delegate void CurrentTrackDelegate(ITurnContext ctx, CurrentTrack currentTrack);
        private CurrentTrack _currentTrack;
        private bool _isValidating;

        public event CurrentTrackDelegate OnCurrentTrackChange;
        private OAuthService _oauthService;
        private ITurnContext _turnContext;

        public CurrentTrackService()
        {
            base.BaseUrl = _endpoint;
        }

        private async void OnCurrentTrackTimerElapsed(object state)
        {
            await Task.Run(async () => {
                var trackToCheck = await GetCurrentTrackObject(_oauthService.AccessToken);

                if (trackToCheck?.Track?.Id != _currentTrack?.Track?.Id && !_isValidating)
                {
                    OnCurrentTrackChange?.Invoke(_turnContext, trackToCheck);
                    _currentTrack = trackToCheck;
                }
            });
        }

        private async Task<string> GetCurrentTrack(AccessToken accessToken)
        {
            var endpoint = $"/currently-playing";
            return await base.Get(endpoint, new AuthenticationHeaderValue(_authenticationSchema, accessToken.Token));
        }

        private async Task<CurrentTrack> GetCurrentTrackObject(AccessToken accessToken)
        {
            var currentlyPlaying = await GetCurrentTrack(accessToken);
            return JsonConvert.DeserializeObject<CurrentTrack>(currentlyPlaying);
        }

        public async Task<CurrentTrack> ValidateTrack()
        {
            _isValidating = true;
            for(int i = 0; i < 4; i++)
            {
                var trackToCheck = await GetCurrentTrackObject(_oauthService.AccessToken);

                if (trackToCheck?.Track?.Id != _currentTrack?.Track?.Id)
                {
                    _currentTrack = trackToCheck;
                    break;
                }
            }
            _isValidating = false;
            return _currentTrack;
        }

        public async Task Initialize(OAuthService oAuthService, ITurnContext ctx)
        {
            _oauthService = oAuthService;
            _turnContext = ctx;
            _currentTrack = await GetCurrentTrackObject(_oauthService.AccessToken);
            var currentTrackTimer = new Timer(OnCurrentTrackTimerElapsed, null, 2000, 2000);
        }
    }
}
