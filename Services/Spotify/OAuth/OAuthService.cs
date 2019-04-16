using Newtonsoft.Json;
using SpotifyPlayer.Models.Spotify;
using SpotifyPlayer.Services.Authorization;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SpotifyPlayer.Services.Spotify.OAuth

{
    public class OAuthService : BaseHttpClient
    {
        private const string _accountsUrl = "https://accounts.spotify.com";
        private const string _redirectUri = "http://localhost:3979/callback";
        private const string _authenticationSchema = "Basic";

        private readonly string _clientId;
        private readonly string _clientSecret;

        private readonly IAuthorizationMapper _authorizationMapper;
        private AccessToken _accessToken;

        public AccessToken AccessToken {

            get {

                if(_accessToken?.ExpiryDate < DateTime.UtcNow)
                {
                    RefreshAccessToken(_accessToken).GetAwaiter().GetResult();
                }

                return _accessToken;
            }
            set {
                this._accessToken = value;
            }
        }

        public OAuthService(IAuthorizationMapper authorizationMapper, string appId, string appSecret)
        {
            base.BaseUrl = _accountsUrl;
            _clientId = appId;
            _clientSecret = appSecret;
            _authorizationMapper = authorizationMapper;
        }
        public async Task SetAccessToken(string key)
        {
            var code = _authorizationMapper.Get(key);

            var context = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", _redirectUri },
            };

            await MakeAccessTokenRequest(context);
        }
        public async Task RefreshAccessToken(AccessToken spotifyAuth)
        {
            var context = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", spotifyAuth.RefreshToken }
            };

            await MakeAccessTokenRequest(context);
        }
        private async Task<AccessToken> MakeAccessTokenRequest(Dictionary<string, string> context)
        {
            var headerKey = ToBase64Encode($"{_clientId}:{_clientSecret}");
            var endpoint = "/api/token/";

            var accessToken = await base.Post(endpoint, new AuthenticationHeaderValue(_authenticationSchema, headerKey), context);
            _accessToken = JsonConvert.DeserializeObject<AccessToken>(accessToken);
            _accessToken.ExpiryDate = DateTime.UtcNow.AddSeconds(_accessToken.Expiry);

            return this.AccessToken;
        }
        public string GetAuthorizeUrl()
        {
            var scope = "user-modify-playback-state,user-read-playback-state";
            var endpoint = $"{_accountsUrl}/authorize/";
            var responseType = "code";

            return $"{endpoint}?client_id={_clientId}&response_type={responseType}&redirect_uri={_redirectUri}&scope={scope}";
        }
        private string ToBase64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
