// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SpotifyPlayer.Bots;
using SpotifyPlayer.Services.Authorization;
using SpotifyPlayer.Services.Spotify.Artists;
using SpotifyPlayer.Services.Spotify.OAuth;
using SpotifyPlayer.Services.Spotify.Player;
using SpotifyPlayer.Services.Spotify.Player.CurrentTrackService;
using SpotifyPlayer.Services.Spotify.Search;
using SpotifyPlayer.Services.Spotify.Tracks;
using SpotifyPlayer.Services.Spotify.UI;
using System;
using System.Linq;

namespace SpotifyPlayer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.ViewLocationFormats.Clear();
                o.ViewLocationFormats.Add("/Callback/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
            });

            var authorizationMapper = new AuthorizationMapper();
            services.AddSingleton(typeof(IAuthorizationMapper), authorizationMapper);
            services.AddBot<SpotifyPlayerBot>(options =>
            {
                var secretKey = Configuration.GetSection("botFileSecret")?.Value;

                var botConfig = BotConfiguration.Load(@".\Bots\SpotifyPlayerBot.bot", secretKey);
                services.AddSingleton(sp => botConfig);

                var service = botConfig.Services.Where(s => s.Type == "endpoint" && s.Name == "development").FirstOrDefault();
                if (!(service is EndpointService endpointService))
                {
                    throw new InvalidOperationException($"The .bot file does not contain a development endpoint.");
                }

                options.CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);

                options.OnTurnError = async (context, exception) =>
                {
                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");
                };
            });

            var dataStore = new MemoryStorage();
            var conversationState = new ConversationState(dataStore);

            var oAuthService = new OAuthService(authorizationMapper, Configuration.GetSection("spotifyAppId")?.Value, Configuration.GetSection("spotifyAppSecret")?.Value);
            var playerService = new PlayerService();
            var currentTrackService = new CurrentTrackService();
            var uiService = new UIService(oAuthService.GetAuthorizeUrl());
            var searchService = new SearchService();
            var artistsService = new ArtistsService();
            var tracksService = new TracksService();

            services.AddSingleton(serviceProvider => {

                var options = serviceProvider.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                var accessors = new SpotifyPlayerBotAccessors(conversationState, 
                                                              oAuthService, 
                                                              playerService, 
                                                              currentTrackService,
                                                              uiService, 
                                                              searchService,
                                                              artistsService,
                                                              tracksService)
                {               
                    ConversationDialogState = conversationState.CreateProperty<DialogState>("DialogState"),
                    OAuthServiceState = conversationState.CreateProperty<OAuthService>("OAuthServiceState"),
                    PlayerServiceState = conversationState.CreateProperty<PlayerService>("PlayerServiceState"),
                    CurrentTrackServiceState = conversationState.CreateProperty<CurrentTrackService>("CurrentServiceState"),
                    UIServiceState = conversationState.CreateProperty<UIService>("UIServiceState"),
                    SearchServiceState = conversationState.CreateProperty<SearchService>("SearchServiceState"),
                    ArtistsServiceState = conversationState.CreateProperty<ArtistsService>("ArtistsServiceState"),
                    TracksServiceState = conversationState.CreateProperty<TracksService>("TracksServiceState")
                };

                return accessors;
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework()
                .UseMvc();
        }
    }
}
