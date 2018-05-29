using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using WeAreDevelopers.Core.Classes;
using WeAreDevelopers.Core.DTO;
using WeAreDevelopers.Core.Interfaces;
using WeAreDevelopers.Luis.Classes;
using WeAreDevelopers.Luis.Interfaces;
using WeAreDevelopers.QnAMaker.Classes;
using WeAreDevelopers.QnAMaker.Interfaces;
using WeAreDevelopersBot.Ressources;


namespace WeAreDevelopersBot.Dialogs
{
    [Serializable]
    public class CallLuisCallStarwarsAPIDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            ILuisRequest LuisAPI = new LuisRequest();
            ILuisProcessing LuisAPIProcessing = new LuisProcessing();          

            JObject LuisResult = await MakeLuisCall(context, activity.Text, LuisAPI);
            TransferObjectLuis ProcessedLuisResult = await LuisAPIProcessing.DoProcessing(context, LuisResult);
        
            if (String.IsNullOrEmpty(ProcessedLuisResult.Name_0))
            {
                await context.PostAsync("There is no droid you are looking for.");
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                var QnAAnswer = await MakeQnAMakerCallforConversion(context, ProcessedLuisResult.Name_0);

                if (QnAAnswer.Score < 0.2)
                {
                    await context.PostAsync($"I am sorry I could not find the name {ProcessedLuisResult.Name_0} in my Database");
                    context.Wait(MessageReceivedAsync);
                }
                else
                {
                    JObject StarwarsAnswer = await CallStarWarsApi(context, QnAAnswer.Answer);
                    await ProcessAndPostToUser(context, ProcessedLuisResult, StarwarsAnswer);

                    context.Wait(MessageReceivedAsync);
                }
            }
        }
        
        private async Task<JObject> MakeLuisCall(IDialogContext context, string text, ILuisRequest LuisAPI)
        {
            var LuisResult = await LuisAPI.MakeLuisRequest(text, KeysAndRessourceStrings.AppIDLuisAPI,
                KeysAndRessourceStrings.SubscriptionKeyLuisAPI);
            return LuisResult;
        }

        private async Task<TransferObjectQnA> MakeQnAMakerCallforConversion(IDialogContext context, string QueryString)

        {
            IQnAMakerRequests QnAMaker = new QnAMakerRequests();
            IQnAMakerMatchHandler qnAMakerMatchHandler = new QnAMakerMatchHandler();
            string KnowledgeBaseId = KeysAndRessourceStrings.KnowledgeBaseIdConvert;
            string SubscriptionKey = KeysAndRessourceStrings.SubscriptionKeyQnAMaker;

            QnAMakerResult QnAMakerResultObject = await QnAMaker.GetQnAMakerResponse(QueryString,
                KnowledgeBaseId, SubscriptionKey);

            TransferObjectQnA returnString = await qnAMakerMatchHandler.QnAMakerResultProcessingWithReturn(context,
                  QueryString, QnAMakerResultObject);

            return returnString;
        }

        private async Task<JObject> CallStarWarsApi(IDialogContext context, string UserQuestion)
        {
            Uri APICallUrl = new Uri("https://swapi.co/api/" + UserQuestion);
            IStarwarsRequest starwars = new StarwarsRequest();
            return await starwars.StarWarsDataRequest(APICallUrl);
        }

        private async Task ProcessAndPostToUser(IDialogContext context, TransferObjectLuis processedLuisResult, JObject starwarsAnswer)
        {
            IMergeAndCreateAnswer merge;
            string ReturnString = String.Empty;

            switch (processedLuisResult.Intent)
            {
                case "birth_year":
                    {
                        merge = new SingleProcessYear();
                        ReturnString = merge.DoMergeAndCreateAnswer(processedLuisResult, starwarsAnswer);
                        break;
                    }
                case "height":
                    {
                        merge = new SingleProcessHeight();
                        ReturnString = merge.DoMergeAndCreateAnswer(processedLuisResult, starwarsAnswer);
                        break;
                    }
                case "mass":
                    {
                        merge = new SingleProcessMass();
                        ReturnString = merge.DoMergeAndCreateAnswer(processedLuisResult, starwarsAnswer);
                        break;
                    }
                case "eye_color":
                    {
                        merge = new SingleProcessEyeColor();
                        ReturnString = merge.DoMergeAndCreateAnswer(processedLuisResult, starwarsAnswer);
                        break;
                    }
                default:
                    {
                        await context.PostAsync("I am sorry i couldn't understand your question");
                    }
                    break;
            }

            await context.PostAsync(ReturnString);
        }

    }
}