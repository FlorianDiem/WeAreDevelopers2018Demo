using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using WeAreDevelopers.Core.Classes;
using WeAreDevelopers.Core.DTO;
using WeAreDevelopers.Core.Interfaces;
using WeAreDevelopers.QnAMaker.Classes;
using WeAreDevelopers.QnAMaker.Interfaces;
using WeAreDevelopersBot.Ressources;

namespace WeAreDevelopersBot.Dialogs
{
    [Serializable]
    public class CallStarwarsAPIDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
                       
            TransferObjectQnA QnAMakerResult = await MakeQnAMakerCallforConversion(context, activity.Text);

            if (QnAMakerResult.Score < 0.2)
            {
                await context.PostAsync(QnAMakerResult.Answer.ToString());
                await context.PostAsync($"I could not find \"{QnAMakerResult.Question}\" in my Database.");
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                JObject StarwarsAnswer = await CallStarWarsApi(context, QnAMakerResult.Answer);                
                await context.PostAsync(StarwarsAnswer.ToString());
                context.Wait(MessageReceivedAsync);
            }          
        }

        private async Task<TransferObjectQnA> MakeQnAMakerCallforConversion(IDialogContext context, string QueryString)
        {
            string KnowledgeBaseId = KeysAndRessourceStrings.KnowledgeBaseIdConvert;
            string SubscriptionKey = KeysAndRessourceStrings.SubscriptionKeyQnAMaker;

            IQnAMakerRequests QnAMakerObject = new QnAMakerRequests();
            IQnAMakerMatchHandler qnAMakerMatchHandler = new QnAMakerMatchHandler();

            // Name to number magic
            QnAMakerResult QnAMakerResultObject = await QnAMakerObject.GetQnAMakerResponse(QueryString, KnowledgeBaseId, SubscriptionKey);
            TransferObjectQnA querystring = await qnAMakerMatchHandler.QnAMakerResultProcessingWithReturn(context, QueryString, QnAMakerResultObject);

            return querystring;
        }

        private async Task<JObject> CallStarWarsApi(IDialogContext context, string ApiIntent)
        {
            Uri APICallUrl = new Uri("https://swapi.co/api/" + ApiIntent);
            IStarwarsRequest StarWarsAPI = new StarwarsRequest();
            return await StarWarsAPI.StarWarsDataRequest(APICallUrl);           
        }
    }
}