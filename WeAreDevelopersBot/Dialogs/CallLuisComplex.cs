using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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
    public class CallLuisComplex : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            ILuisRequest LuisAPI = new LuisRequest();
            ILuisProcessing LuisAPIProcessing = new LuisProcessing();
            var activity = await result as Activity;

            var LuisResult = await MakeLuisCall(context, activity.Text, LuisAPI);

            var ProcessedLuisResult = await LuisAPIProcessing.DoProcessingComplex(context, LuisResult);
            
            if ((ProcessedLuisResult.Name_0 == "") && (ProcessedLuisResult.Name_1 == ""))
            {
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                var QnAAnswer = await MakeNameQnAMakerCall(context, ProcessedLuisResult);

                if (QnAAnswer[0].Score < 0.2)
                {
                    await context.PostAsync("I am sorry I could not find the name " + ProcessedLuisResult.Name_0 + " in my Database");
                    context.Wait(MessageReceivedAsync);
                }
                else if(QnAAnswer[1].Score < 0.2)
                {
                    await context.PostAsync("I am sorry I could not find the name " + ProcessedLuisResult.Name_1 + " in my Database");
                    context.Wait(MessageReceivedAsync);
                }
                else if((QnAAnswer[1].Score < 0.2)&& (QnAAnswer[0].Score < 0.2))
                {
                    await context.PostAsync("I am sorry I could not find neither name "
                        + ProcessedLuisResult.Name_0 + " nor the name "
                        + ProcessedLuisResult.Name_0 + " in my Database");
                    context.Wait(MessageReceivedAsync);
                }
                else
                {
                    JObject StarwarsAnswer1 = await CallStarWarsApi(context, QnAAnswer[0].Answer);
                    JObject StarwarsAnswer2 = await CallStarWarsApi(context, QnAAnswer[1].Answer);

                    await ProcessAndPostToUser(context, ProcessedLuisResult, StarwarsAnswer1,StarwarsAnswer2);

                    context.Wait(MessageReceivedAsync);
                }
            }
        }

        private async Task ProcessAndPostToUser(IDialogContext context, TransferObjectLuis processedLuisResult, JObject starwarsAnswer,JObject starwarsanswer2)
        {
            IMergeAndCreateAnswer merge;
            string ReturnString = String.Empty;

            switch (processedLuisResult.Intent)
            {
                case "birth_year":
                    {
                        merge = new PluralProcessYear();
                        ReturnString = merge.DoMergeAndCreateAnswer(processedLuisResult, starwarsAnswer, starwarsanswer2);
                        break;
                    }
                case "height":
                    {
                        merge = new PluralProcessHeight();
                        ReturnString = merge.DoMergeAndCreateAnswer(processedLuisResult, starwarsAnswer,starwarsanswer2);
                        break;
                    }
                case "mass":
                    {
                        merge = new PluralProcessMass();
                        ReturnString = merge.DoMergeAndCreateAnswer(processedLuisResult, starwarsAnswer, starwarsanswer2);
                        break;
                    }
                default:
                    {
                        await context.PostAsync("I am sorry i couldn't understand your question");
                        ReturnString = "";
                    }
                    break;
            }

            await context.PostAsync(ReturnString);
        }

        private async Task<JObject> MakeLuisCall(IDialogContext context, string text, ILuisRequest LuisAPI)
        {
            var LuisResult = await LuisAPI.MakeLuisRequest(text, "0ae7e86b-8e6c-4bd0-b245-674af8350fe8", "64cc7762ef09403c8abf4873a8ba05c5");
            return LuisResult;
        }

        private async Task<List<TransferObjectQnA>> MakeNameQnAMakerCall(IDialogContext context, TransferObjectLuis QueryString)
        {
            IQnAMakerRequests QnAMaker = new QnAMakerRequests();
            IQnAMakerMatchHandler qnAMakerMatchHandler = new QnAMakerMatchHandler();
            List<TransferObjectQnA> TransferList = new List<TransferObjectQnA>();

            string KnowledgeBaseId = KeysAndRessourceStrings.KnowledgeBaseIdConvert;
            string SubscriptionKey = KeysAndRessourceStrings.SubscriptionKeyQnAMaker;
            
            QnAMakerResult QnAMakerResultObject = await QnAMaker.GetQnAMakerResponse(QueryString.Name_0,
                KnowledgeBaseId, SubscriptionKey);

            TransferList.Add(await qnAMakerMatchHandler.QnAMakerResultProcessingWithReturn(context,
                  QueryString.Name_0, QnAMakerResultObject));

            QnAMakerResult QnAMakerResultObject2 = await QnAMaker.GetQnAMakerResponse(QueryString.Name_1,
                KnowledgeBaseId, SubscriptionKey);

            TransferList.Add(await qnAMakerMatchHandler.QnAMakerResultProcessingWithReturn(context,
                  QueryString.Name_1, QnAMakerResultObject));

            return TransferList;
        }

        private async Task<JObject> CallStarWarsApi(IDialogContext context, string UserQuestion)
        {
            Uri APICallUrl = new Uri("https://swapi.co/api/" + UserQuestion);
            IStarwarsRequest starwars = new StarwarsRequest();
            return await starwars.StarWarsDataRequest(APICallUrl);
        }

    }
}