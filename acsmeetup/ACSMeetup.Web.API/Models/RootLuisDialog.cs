using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace ACSMeetup.Web.API.Models
{
    [LuisModel("<Luis Model ID>", "<Azure Luis Subscription Key>")]
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Say 'help' if you need assistance.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Welcome")]
        [LuisIntent("Help")]
        [LuisIntent("ReadNews")]
        [LuisIntent("Question")]
        public async Task Feed(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            string device = "cortana";
            NewsBotManager newsBot = new NewsBotManager();
            var message = newsBot.GetNewsLinesByIntent(LuisResultToStdLuis(result), device);
            MessageOptions options = new MessageOptions
            {
                InputHint = InputHints.ExpectingInput
            };
            await context.SayAsync(message.text, message.SSML, options);
            context.Wait(this.MessageReceived);
        }

        private async Task<string> GetUserId(IAwaitable<IMessageActivity> activity)
        {
            var inmessage = await activity;

            try
            {
                return inmessage.From.Id;
            }
            catch
            {
                return "TBD";
            }
        }

        private LUIS LuisResultToStdLuis(LuisResult result)
        {
            return new LUIS
            {
                topScoringIntent = new LUISIntent
                {
                    intent = result.TopScoringIntent.Intent,
                    score = result.TopScoringIntent.Score
                },
                entities = (from t in result.Entities
                            select new LUISEntity
                            {
                                entity = t.Entity,
                                score = t.Score.Value,
                                type = t.Type,
                                startIndex = t.StartIndex,
                                endIndex = t.EndIndex
                            }).ToArray(),
                query = result.Query
            };
        }
    }
}