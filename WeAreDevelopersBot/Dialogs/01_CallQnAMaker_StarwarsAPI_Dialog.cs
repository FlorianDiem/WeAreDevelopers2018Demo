using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;
using WeAreDevelopers.QnAMaker.Classes;
using WeAreDevelopers.QnAMaker.Interfaces;
using WeAreDevelopersBot.Ressources;

namespace WeAreDevelopersBot.Dialogs
{
    [Serializable]
    public class CallStarwarsQnADialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            await MakeQnAMakerCall(context, activity.Text);

            context.Wait(MessageReceivedAsync);
        }

        private async Task MakeQnAMakerCall(IDialogContext context, string QueryString)
        {
            string KnowledgeBaseId = KeysAndRessourceStrings.KnowledgeBaseIdQuestions;
            string SubscriptionKey = KeysAndRessourceStrings.SubscriptionKeyQnAMaker;

            IQnAMakerRequests QnAMakerAPI = new QnAMakerRequests();
            IQnAMakerMatchHandler QnAMakerMatchHandler = new QnAMakerMatchHandler();

            QnAMakerResult QnAMakerResultObject = await QnAMakerAPI.GetQnAMakerResponse(QueryString, KnowledgeBaseId, SubscriptionKey);

            await QnAMakerMatchHandler.QnAMakerResultProcessing(context, QueryString, QnAMakerResultObject);
        }
    }
}