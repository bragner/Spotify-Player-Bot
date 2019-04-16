namespace SpotifyPlayer.Services.Authorization
{
    public interface IAuthorizationMapper
    {
        string Get(string key);
        string Set(string code);
    }
}