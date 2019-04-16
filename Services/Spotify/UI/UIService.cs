using AdaptiveCards;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using SpotifyPlayer.Models.Bot;
using SpotifyPlayer.Models.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpotifyPlayer.Services.Spotify.UI
{
    public class UIService
    {
        private readonly string _authLink;

        public UIService(string authLink)
        {
            this._authLink = authLink;
        }
        public Attachment GetSpotifyCard(CurrentTrack result = null, SearchResult searchResult = null)
        {
            var spotifyCard = new AdaptiveCard("1.0");

            if (searchResult != null)
                GetSearchResultCard(ref spotifyCard, searchResult);

            if (result != null)
                GetTrackPreviewCard(ref spotifyCard, result);

            GetSearchBoxArea(ref spotifyCard);

            return new Attachment { Content = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(spotifyCard)), ContentType = AdaptiveCard.ContentType };
        }
        private void GetSearchResultCard(ref AdaptiveCard spotifyCard, SearchResult searchResult)
        {
            if (searchResult != null)
            {
                if(searchResult.Artists?.Artists?.Count > 0)
                {
                    spotifyCard.Body.Add(new AdaptiveTextBlock
                    {
                        Text = $"Artists",
                        Weight = AdaptiveTextWeight.Lighter,
                        Size = AdaptiveTextSize.ExtraLarge,
                        FontStyle = AdaptiveFontStyle.Monospace
                    });
                    foreach (var artist in searchResult.Artists.Artists.Take(5))
                    {
                        spotifyCard.Body.Add(new AdaptiveContainer
                        {
                            Items = new List<AdaptiveElement>
                            {
                                new AdaptiveTextBlock
                                {
                                    Text = $"{artist.Name}",
                                    Size = AdaptiveTextSize.Medium
                                }
                            },
                            SelectAction = new AdaptiveSubmitAction
                            {
                                DataJson = JsonConvert.SerializeObject(new InputAction
                                {
                                    Action = "artist",
                                    Context = artist.Id
                                })
                            }

                        });
                    }
                }
                spotifyCard.Body.Add(new AdaptiveTextBlock
                {
                    Text = $"Tracks",
                    Weight = AdaptiveTextWeight.Lighter,
                    Size = AdaptiveTextSize.ExtraLarge,
                    FontStyle = AdaptiveFontStyle.Monospace,
                    Separator = true,

                });
                foreach (var track in searchResult.Tracks.Tracks.Take(5))
                {
                    spotifyCard.Body.Add(new AdaptiveColumnSet
                    {
                        Columns = new List<AdaptiveColumn>
                        {
                            new AdaptiveColumn
                            {
                                VerticalContentAlignment = AdaptiveVerticalContentAlignment.Center,
                                Width = "80",
                                Items = new List<AdaptiveElement>
                                {
                                    new AdaptiveContainer
                                    {
                                        Items = new List<AdaptiveElement>
                                        {
                                            new AdaptiveTextBlock
                                            {
                                                Text = $"{track.Name}",
                                                Size = AdaptiveTextSize.Medium
                                            },
                                            new AdaptiveTextBlock
                                            {
                                                Text = $"{track.Artists.FirstOrDefault().Name} - *{track.Album.Name}*",
                                                Weight = AdaptiveTextWeight.Lighter,
                                                Size = AdaptiveTextSize.Small
                                            },
                                        },
                                        SelectAction = new AdaptiveSubmitAction
                                        {
                                            DataJson = JsonConvert.SerializeObject(new InputAction
                                            {
                                                Action = "track",
                                                Context = track.Id
                                            })
                                        }
                                    }
                                }
                            },
                            new AdaptiveColumn
                            {
                                 VerticalContentAlignment = AdaptiveVerticalContentAlignment.Center,
                                 Items = new List<AdaptiveElement>
                                 {
                                     new AdaptiveContainer
                                     {
                                         Items = new List<AdaptiveElement>
                                         {
                                             new AdaptiveImage
                                             {
                                                 Url = new Uri(track.Album.Images.LastOrDefault().Url),
                                                 Style = AdaptiveImageStyle.Person,
                                                 HorizontalAlignment = AdaptiveHorizontalAlignment.Right
                                             }
                                         },
                                     }
                                 },
                                 Width = "20"
                            }
                        }
                    });
                }
            }
        }
        private void GetTrackPreviewCard(ref AdaptiveCard spotifyCard, CurrentTrack trackResult)
        {
            spotifyCard.Body.Add(new AdaptiveColumnSet
            {
                Spacing = AdaptiveSpacing.Medium,
                VerticalContentAlignment = AdaptiveVerticalContentAlignment.Center,
                Separator = true,
                Columns = new List<AdaptiveColumn>
                {
                    new AdaptiveColumn
                    {
                        VerticalContentAlignment = AdaptiveVerticalContentAlignment.Center,
                        Items = new List<AdaptiveElement>
                        {

                            new AdaptiveImage
                            {
                                HorizontalAlignment = AdaptiveHorizontalAlignment.Center,
                                Url = new Uri(trackResult.Track.Album.Images[1].Url),
                                Size = AdaptiveImageSize.Large
                            },
                        },
                        Width="2"
                    },
                    new AdaptiveColumn
                    {
                        Items = new List<AdaptiveElement>
                        {
                            new AdaptiveContainer
                            {
                                Items = new List<AdaptiveElement>{
                                    new AdaptiveTextBlock
                                    {
                                        Text = trackResult.Track.Name,
                                        Size = AdaptiveTextSize.Large,
                                        Weight = AdaptiveTextWeight.Bolder,
                                        Wrap = true,
                                    },
                                    new AdaptiveTextBlock
                                    {
                                        Text = trackResult.Track.Artists.FirstOrDefault().Name,
                                        Size = AdaptiveTextSize.Large,
                                        Weight = AdaptiveTextWeight.Lighter,
                                        FontStyle = AdaptiveFontStyle.Monospace,
                                        IsSubtle = true
                                    },
                                    new AdaptiveTextBlock
                                    {
                                        Text = $"*{trackResult.Track.Album.Name}* ({trackResult.Track.Album.RealeseDate})",
                                        Size = AdaptiveTextSize.Medium,
                                        Weight = AdaptiveTextWeight.Lighter,
                                        FontStyle = AdaptiveFontStyle.Monospace,
                                        IsSubtle = true
                                    }
                                    }
                            },
                            GetPlayerControl()
                        },
                        Width="2"
                    }
                }
            });
            
        }

        internal Attachment GetStartPlayerCard()
        {
            var spotifyCard = new AdaptiveCard("1.0");

            spotifyCard.Body.Add(new AdaptiveTextBlock
            {
                Text = $"Looks like you don't have Spotify running.{Environment.NewLine}Not to worry, click this button to start listening to my favorite song😊"
            });
            spotifyCard.Actions.Add(new AdaptiveOpenUrlAction
            {
                Title = "Open Spotify in browser",
                Url = new Uri("https://open.spotify.com/track/4qyKcYZpN4Itz0pl7CUIYY")

            });

            return new Attachment { Content = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(spotifyCard)), ContentType = AdaptiveCard.ContentType };
        }
        internal Attachment GetLoginCard()
        {
            var spotifyCard = new AdaptiveCard("1.0");

            spotifyCard.Body.Add(new AdaptiveTextBlock
            {
                Wrap = true,
                Text = $"To get started we just need to follow a few steps😊{Environment.NewLine}" +
                                                     $"1. Click the button below{Environment.NewLine}" +
                                                     $"2. Sign in to your Spotify account (You got Premium right?){Environment.NewLine}" +
                                                     $"3. Give me the code you'll receive once the registration process is completed{Environment.NewLine}{Environment.NewLine}" +
                                                     $"Easy right? Here's the button I was talking about "
            });
            spotifyCard.Actions.Add(new AdaptiveOpenUrlAction
            {
                Title = "Let's get started!",
                Url = new Uri(_authLink)

            });

            return new Attachment { Content = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(spotifyCard)), ContentType = AdaptiveCard.ContentType };
        }

        private AdaptiveElement GetPlayerControl()
        {
            var repeat = new AdaptiveChoiceSetInput
            {
                Id = "Repeat",
                Style = AdaptiveChoiceInputStyle.Compact,
                IsMultiSelect = true,
                
                Choices = new List<AdaptiveChoice>
                                        {
                                            new AdaptiveChoice
                                            {
                                                Title = "🔁",
                                                Value = "track"
                                            }
                                        },
            };
            var shuffle = new AdaptiveChoiceSetInput
            {
                Id = "Shuffle",
                Style = AdaptiveChoiceInputStyle.Compact,
                IsMultiSelect = true,
                Choices = new List<AdaptiveChoice>
                {
                    new AdaptiveChoice
                    {
                        Title = "🔀",
                        Value = "true"
                    }
                },
            };

            return new AdaptiveContainer
            {
                VerticalContentAlignment = AdaptiveVerticalContentAlignment.Center,
                Items = new List<AdaptiveElement>
                {
                    new AdaptiveColumnSet
                    {
                        Columns = new List<AdaptiveColumn>
                        {
                            new AdaptiveColumn
                            {
                                SelectAction = new AdaptiveSubmitAction
                                {
                                        DataJson = JsonConvert.SerializeObject(new InputAction
                                        {
                                            Action = "previous"
                                        })
                                },
                                Items = new List<AdaptiveElement>
                                {
                                    new AdaptiveTextBlock
                                    {
                                        Text = "⏮",
                                        Size = AdaptiveTextSize.Large,
                                        HorizontalAlignment = AdaptiveHorizontalAlignment.Center
                                    }
                                }
                            },
                            new AdaptiveColumn
                            {
                                SelectAction = new AdaptiveSubmitAction
                                {
                                        DataJson = JsonConvert.SerializeObject(new InputAction
                                        {
                                            Action = "pause"
                                        })
                                },
                                Items = new List<AdaptiveElement>
                                {
                                    new AdaptiveTextBlock
                                    {
                                        Text = "⏸",
                                        Size = AdaptiveTextSize.Large,
                                        HorizontalAlignment = AdaptiveHorizontalAlignment.Center
                                    }
                                }
                            },
                            new AdaptiveColumn
                            {
                                SelectAction = new AdaptiveSubmitAction
                                {
                                        Id = "player",
                                        DataJson = JsonConvert.SerializeObject(new InputAction
                                        {
                                            Action = "play"
                                        })
                                },
                                Items = new List<AdaptiveElement>
                                {
                                    new AdaptiveTextBlock
                                    {
                                        Text = "▶",
                                        Size = AdaptiveTextSize.Large,
                                        HorizontalAlignment = AdaptiveHorizontalAlignment.Center
                                    }
                                },
                            },
                            new AdaptiveColumn
                            {
                                SelectAction = new AdaptiveSubmitAction
                                {
                                    DataJson = JsonConvert.SerializeObject(new InputAction
                                    {
                                        Action = "next"
                                    })
                                },
                                Items = new List<AdaptiveElement>
                                {
                                    new AdaptiveTextBlock
                                    {
                                        Text = "⏭",
                                        Size = AdaptiveTextSize.Large,
                                        HorizontalAlignment = AdaptiveHorizontalAlignment.Center
                                    }
                                },
                            },
                            new AdaptiveColumn
                            {

                            },
                            new AdaptiveColumn
                            {
                                SelectAction = new AdaptiveSubmitAction
                                {
                                    DataJson = JsonConvert.SerializeObject(new InputAction
                                    {
                                        Action = "playerSettings"
                                    })
                                },
                                Items = new List<AdaptiveElement>
                                {
                                    shuffle,
                                    repeat
                                }
                            }
                        }
                    }
                }
            };
        }

        private void GetSearchBoxArea(ref AdaptiveCard spotifyCard)
        {
            var searchAction = new AdaptiveTextInput
            {
                Id = "Context",
                Placeholder = "Search...",
                Style = AdaptiveTextInputStyle.Text,
                Height = AdaptiveHeight.Stretch
            };
            spotifyCard.Actions.Add(new AdaptiveShowCardAction
            {
                Title = "Search",
                Card = new AdaptiveCard("1.0")
                {
                    VerticalContentAlignment = AdaptiveVerticalContentAlignment.Center,
                    Body = new List<AdaptiveElement>
                    {
                        new AdaptiveColumnSet
                        {
                            Columns = new List<AdaptiveColumn>
                            {
                                new AdaptiveColumn
                                {
                                    Items = new List<AdaptiveElement>
                                    {
                                        searchAction
                                    },
                                    Width="2"
                                },
                                new AdaptiveColumn
                                {
                                    SelectAction = new AdaptiveSubmitAction
                                    {
                                        DataJson = JsonConvert.SerializeObject(new InputAction
                                        {
                                            Action = "search"
                                        })
                                    },
                                    Items = new List<AdaptiveElement>
                                    {
                                        new AdaptiveTextBlock
                                        {
                                            Text = "✔",
                                            Size = AdaptiveTextSize.Large,
                                            HorizontalAlignment = AdaptiveHorizontalAlignment.Center
                                        }
                                    },
                                    Width="2"
                                }
                            }
                        }
                    }
                }
            });
        }
    }
}
