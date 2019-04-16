// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using SpotifyPlayer.Dialogs.Login;
using SpotifyPlayer.Models.Spotify;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpotifyPlayer.Bots
{
    public class SpotifyPlayerBot : IBot
    {
        private readonly SpotifyPlayerBotAccessors _accessors;
        private readonly DialogSet _dialogs;
        private const string PlayerDialog = "playerDialog";
        private const string ShowPlayerDialog = "showPlayerDialog";
        private const string PlayerPrompt = "playerPrompt";

        public SpotifyPlayerBot(SpotifyPlayerBotAccessors accessors)
        {
            _accessors = accessors ?? throw new ArgumentNullException(nameof(accessors));

            _dialogs = new DialogSet(_accessors.ConversationDialogState);

            var waterfallSteps = new WaterfallStep[]
            {
                Greeting,
                AccessStep,
            };
            var waterfallStepsLoop = new WaterfallStep[]
            {
                ShowPlayer,
                HandleUserInput,
            };

            _dialogs.Add(new WaterfallDialog(PlayerDialog, waterfallSteps));
            _dialogs.Add(new WaterfallDialog(ShowPlayerDialog, waterfallStepsLoop));
            _dialogs.Add(new LoginDialog(LoginDialog.ID, _accessors.OAuthService, _accessors.UIService));
            _dialogs.Add(new TextPrompt(PlayerPrompt));
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            if (turnContext.Activity.Type == ActivityTypes.Message || turnContext.Activity.MembersAdded.FirstOrDefault().Name != "Bot")
            {
                if (turnContext.Activity.Value != null)
                    turnContext.Activity.Text = "action";

                var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
                var results = await dialogContext.ContinueDialogAsync(cancellationToken);

                if (results.Status == DialogTurnStatus.Empty)
                {
                    await dialogContext.BeginDialogAsync(PlayerDialog, null, cancellationToken);
                }
                else if (results.Status == DialogTurnStatus.Complete)
                {
                    if (results.Result != null)
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text($"Bye!"));
                    }
                }
                await _accessors.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            }
        }
        private async Task<DialogTurnResult> HandleUserInput(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            dynamic activity = null;
            string action = null;

            if (stepContext.Context.Activity.Value != null)
            {
                activity = stepContext.Context.Activity.Value;
                action = activity.Action.ToString();
            }
            else if(!string.IsNullOrEmpty(stepContext.Context.Activity.Text))
            {
                var userTextInput = stepContext.Context.Activity.Text.Trim();
                var commands = userTextInput.Split(" ", 2);
                activity = new { Context = commands.LastOrDefault() };
                action = commands.FirstOrDefault();
            }

            var options = await ExecuteUserInput(action, activity);

            return await stepContext.ReplaceDialogAsync(ShowPlayerDialog, options, cancellationToken);
        }

        private async Task<object> ExecuteUserInput(string action, dynamic activity)
        {
            object options = null;
            var accessToken = _accessors.OAuthService.AccessToken;
            switch (action)
            {
                case "play":
                    await _accessors.PlayerService.Play(accessToken);
                    break;
                case "pause":
                    await _accessors.PlayerService.Pause(accessToken);
                    break;
                case "next":
                    options = await _accessors.PlayerService.Next(accessToken);
                    break;
                case "previous":
                    options = await _accessors.PlayerService.Previous(accessToken);
                    break;
                case "track":
                    {
                        var ctx = activity.Context.ToString();
                        var trackJson = await _accessors.TracksService.GetTrack(accessToken, ctx);
                        var track = JsonConvert.DeserializeObject<Track>(trackJson);
                        options = await _accessors.PlayerService.PlayTrack(accessToken,
                            JsonConvert.SerializeObject(new
                            {
                                context_uri = track.Album.Uri,
                                offset = new { uri = track.Uri }
                            }));
                        break;
                    }
                case "artist":
                    {
                        var ctx = activity.Context.ToString();
                        var artistTopTracks = await _accessors.ArtistsService.GetArtistTopTracks(accessToken, ctx);
                        var tracksResult = JsonConvert.DeserializeObject<ArtistTopTracks>(artistTopTracks);

                        options = JsonConvert.SerializeObject(new SearchResult
                        {
                            Tracks = new TracksResult
                            {
                                Tracks = tracksResult.Tracks
                            }
                        });
                        break;
                    }
                case "playerSettings":
                    {
                        var shuffle = activity.Shuffle?.ToString() ?? "false";
                        var repeat = activity.Repeat?.ToString() ?? "off";
                        await _accessors.PlayerService.Shuffle(accessToken, shuffle);
                        await _accessors.PlayerService.Repeat(accessToken, repeat);
                        break;
                    }
                case "search":
                    {
                        var query = activity.Context.ToString();
                        options = await _accessors.SearchService.Search(accessToken, query);
                        break;
                    }
            }
            return options;
        }

        private async Task<DialogTurnResult> ShowPlayer(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        { 
            SearchResult search = null;
            if (stepContext.Options != null)
            {
                if (!bool.TryParse(stepContext.Options.ToString(), out _))
                {
                    var json = stepContext.Options.ToString();
                    search = JsonConvert.DeserializeObject<SearchResult>(json);
                }
                var track = await _accessors.CurrentTrackService.ValidateTrack();
                Attachment card;
                if(track != null)
                    card = _accessors.UIService.GetSpotifyCard(track, search);
                else
                    card = _accessors.UIService.GetStartPlayerCard();

                return await stepContext.PromptAsync(PlayerPrompt, new PromptOptions { Prompt = MessageFactory.Attachment(card) as Activity }, cancellationToken);
            }
            return await stepContext.PromptAsync(PlayerPrompt, new PromptOptions(), cancellationToken);
        }

        private async Task<DialogTurnResult> AccessStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var activity = stepContext.Context.Activity.CreateReply();
            activity.Text = "You did it!";
            await stepContext.Context.SendActivityAsync(activity);

            await _accessors.CurrentTrackService.Initialize(_accessors.OAuthService, stepContext.Context);
            _accessors.CurrentTrackService.OnCurrentTrackChange += OnCurrentTrackTimer;
            return await stepContext.BeginDialogAsync(ShowPlayerDialog, true, cancellationToken);
        }

        private async Task<DialogTurnResult> Greeting(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var activity = stepContext.Context.Activity.CreateReply();
            activity.Text = "Hi and welcome to Spotify Bot Player!";
            await stepContext.Context.SendActivityAsync(activity);
            return await stepContext.BeginDialogAsync(LoginDialog.ID, null, cancellationToken);
        }

        private async void OnCurrentTrackTimer(ITurnContext ctx, CurrentTrack currentTrack)
        {
            var spotifyCard = _accessors.UIService.GetSpotifyCard(currentTrack);

            await ctx.SendActivityAsync(MessageFactory.Attachment(spotifyCard) as Activity);
        }
    }
}
