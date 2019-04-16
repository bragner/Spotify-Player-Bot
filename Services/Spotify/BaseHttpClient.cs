using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyPlayer.Services.Spotify
{
    public class BaseHttpClient
    {
        public string BaseUrl = string.Empty;
        public async Task<string> Get(string endpoint, AuthenticationHeaderValue authenticationHeader)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = authenticationHeader;
                var response = await client.GetAsync($"{BaseUrl}{endpoint}");
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
        }
        public async Task<bool> Put(string endpoint, AuthenticationHeaderValue authenticationHeader, string context = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = authenticationHeader;
                var response = await client.PutAsync($"{BaseUrl}{endpoint}", context == null ? null : new StringContent(context, Encoding.UTF8, "application/json"));
                var result = await response.Content.ReadAsStringAsync();
                return response.IsSuccessStatusCode;
            }
        }
        public async Task<dynamic> Post(string endpoint, AuthenticationHeaderValue authenticationHeader, Dictionary<string, string> context = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = authenticationHeader;
                var response = await client.PostAsync($"{BaseUrl}{endpoint}", context == null ? null : new FormUrlEncodedContent(context));
                var result = await response.Content.ReadAsStringAsync();

                if (context != null)
                    return result;

                return response.IsSuccessStatusCode;
            }
        }
        public async Task Delete(string endpoint, AuthenticationHeaderValue authenticationHeader)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = authenticationHeader;
                var response = await client.DeleteAsync($"{BaseUrl}{endpoint}");
                var result = await response.Content.ReadAsStringAsync();
            }
        }
    }
}
