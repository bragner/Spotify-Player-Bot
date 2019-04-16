using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using SpotifyPlayer.Services.Spotify.OAuth;
using SpotifyPlayer.Services.Spotify.UI;
using System.Threading;
using System.Threading.Tasks;

namespace SpotifyPlayer.Dialogs.Login
{
    public class LoginDialog : ComponentDialog
    {
        public const string ID = "loginDialog";

        private const string SendOAuthLinkPrompt = "oAuthlink";
        private const string RecieveCodeForValidationPrompt = "code";

        private readonly OAuthService _oAuthService;
        private readonly UIService _uiService;

        public LoginDialog(string id, OAuthService oAuthService, UIService uiService) : base(id)
        {
            _oAuthService = oAuthService;
            _uiService = uiService;

            var waterfallSteps = new WaterfallStep[]
            {
                SendLinkStepAsync,
                GetKeyStepAsync
            };

            AddDialog(new WaterfallDialog(id, waterfallSteps));
            AddDialog(new TextPrompt(SendOAuthLinkPrompt));
            AddDialog(new TextPrompt(RecieveCodeForValidationPrompt));
        }

        private async Task<DialogTurnResult> GetKeyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if(stepContext.Result != null)
            {
                await _oAuthService.SetAccessToken(stepContext.Result.ToString());
                return await stepContext.EndDialogAsync();
            }

            return await stepContext.ReplaceDialogAsync(SendOAuthLinkPrompt);
        }

        private async Task<DialogTurnResult> SendLinkStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var card = _uiService.GetLoginCard();
            return await stepContext.PromptAsync(SendOAuthLinkPrompt, 
                                                 new PromptOptions
                                                 {
                                                     Prompt = MessageFactory.Attachment(card) as Activity
                                                 }, 
                                                 cancellationToken);
        }
    }
}
