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
    public class CallLuisCallStarwarsAPIorQnADialog : IDialog<object>
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
            IQnAMakerRequests QnAMakerAPI = new QnAMakerRequests();

            string KnowledgeBaseId = KeysAndRessourceStrings.KnowledgeBaseIdQuestions;
            string SubscriptionKey = KeysAndRessourceStrings.SubscriptionKeyQnAMaker;

            QnAMakerResult QnAMakerResultObject = await QnAMakerAPI.GetQnAMakerResponse(activity.Text,
                KnowledgeBaseId, SubscriptionKey);

            JObject LuisResult = await MakeLuisCall(context, activity.Text, LuisAPI);

            if (await LuisQnADecision(QnAMakerResultObject, LuisResult))
            {
                await LuisCallProcess(context, result, LuisResult);
            }
            else
            {
                await QnACallProcess(context, result, activity.Text, QnAMakerResultObject);
            }
        }

        private async Task QnACallProcess(IDialogContext context, IAwaitable<object> result, string QueryString, QnAMakerResult QnAMakerResult)
        {
            IQnAMakerMatchHandler qnAMakerMatchHandler = new QnAMakerMatchHandler();

            await qnAMakerMatchHandler.QnAMakerResultProcessing(context, QueryString, QnAMakerResult);
        }

        private async Task LuisCallProcess(IDialogContext context, IAwaitable<object> result, JObject LuisResult)
        {
            ILuisProcessing LuisAPIProcessing = new LuisProcessing();

            var ProcessedLuisResult = await LuisAPIProcessing.DoProcessing(context, LuisResult);
            string StringName = ProcessedLuisResult.Name_0;

            if (StringName == "")
            {
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                var QnAAnswer = await MakeNameQnAMakerCall(context, StringName);

                if (QnAAnswer.Score < 0.2)
                {
                    await context.PostAsync("I am sorry I could not find the name " + StringName + " in my Database");
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

        private async Task<bool> LuisQnADecision(QnAMakerResult QnAMakerResultObject, JObject LuisResult)
        {
            double QnAScore = QnAMakerResultObject.Answers[0].Score;
            double LuisScore = LuisResult.Value<JObject>("topScoringIntent").Value<double>("score");
            if (QnAScore > LuisScore*100)
                return false;
            else
                return true;
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

        private async Task<JObject> MakeLuisCall(IDialogContext context, string text, ILuisRequest LuisAPI)
        {
            var LuisResult = await LuisAPI.MakeLuisRequest(text, KeysAndRessourceStrings.AppIDLuisAPI,
                KeysAndRessourceStrings.SubscriptionKeyLuisAPI);
            return LuisResult;
        }

        private async Task<TransferObjectQnA> MakeNameQnAMakerCall(IDialogContext context, string QueryString)

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

    }
}