using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using SpotifyPlayer.Services.Spotify.Artists;
using SpotifyPlayer.Services.Spotify.OAuth;
using SpotifyPlayer.Services.Spotify.Player;
using SpotifyPlayer.Services.Spotify.Player.CurrentTrackService;
using SpotifyPlayer.Services.Spotify.Search;
using SpotifyPlayer.Services.Spotify.Tracks;
using SpotifyPlayer.Services.Spotify.UI;
using System;

namespace SpotifyPlayer.Bots
{
    public class SpotifyPlayerBotAccessors
    {
        public SpotifyPlayerBotAccessors(ConversationState conversationState, 
                                         OAuthService oAuthService, 
                                         PlayerService playerService,
                                         CurrentTrackService currentTrackService,
                                         UIService uiService, 
                                         SearchService searchService,
                                         ArtistsService artistsService,
                                         TracksService tracksService)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            OAuthService = oAuthService ?? throw new ArgumentNullException(nameof(oAuthService));
            PlayerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            CurrentTrackService = currentTrackService ?? throw new ArgumentNullException(nameof(currentTrackService));
            UIService = uiService ?? throw new ArgumentNullException(nameof(uiService));
            SearchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
            ArtistsService = artistsService ?? throw new ArgumentNullException(nameof(artistsService));
            TracksService = tracksService ?? throw new ArgumentNullException(nameof(tracksService));
        }
        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }
        public IStatePropertyAccessor<OAuthService> OAuthServiceState { get; set; }
        public IStatePropertyAccessor<PlayerService> PlayerServiceState { get; set; }
        public IStatePropertyAccessor<CurrentTrackService> CurrentTrackServiceState { get; set; }
        public IStatePropertyAccessor<UIService> UIServiceState { get; set; }
        public IStatePropertyAccessor<SearchService> SearchServiceState { get; set; }
        public IStatePropertyAccessor<ArtistsService> ArtistsServiceState { get; set; }
        public IStatePropertyAccessor<TracksService> TracksServiceState { get; set; }


        public ConversationState ConversationState { get; set; }
        public OAuthService OAuthService { get; set; }
        public PlayerService PlayerService { get; set; }
        public CurrentTrackService CurrentTrackService { get; set; }
        public UIService UIService { get; set; }
        public SearchService SearchService { get; set; }
        public ArtistsService ArtistsService { get; set; }
        public TracksService TracksService { get; set; }

    }
}
