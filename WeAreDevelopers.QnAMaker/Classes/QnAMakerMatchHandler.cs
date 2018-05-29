using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeAreDevelopers.QnAMaker.Interfaces;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;
using WeAreDevelopers.Core.DTO;

namespace WeAreDevelopers.QnAMaker.Classes
{
    public class QnAMakerMatchHandler:IQnAMakerMatchHandler
    {
        public async Task QnAMakerResultProcessing(IDialogContext context, string QueryString, QnAMakerResult QnAMakerResultObject)
        {
            if (QnAMakerResultObject.Answers.First().Score == 0)
            {
                await NoMatchHandler(context, QueryString);
            }
            else
            {
                await DefaultMatchHandler(context, QueryString, QnAMakerResultObject);
            }
        }

        public async Task<TransferObjectQnA> QnAMakerResultProcessingWithReturn(IDialogContext context, string QueryString, QnAMakerResult QnAMakerResultObject)
        {
            if (QnAMakerResultObject.Answers.First().Score < 0.3)
            {
                return await NoMatchHandlerWithReturn(context, QueryString);
            }
            else
            {
                return await DefaultMatchHandlerWithReturn(context, QueryString, QnAMakerResultObject);
            }
        }

        private async Task NoMatchHandler(IDialogContext context, string queryString)
        {
            var message = context.MakeMessage();
            message.Text = Ressources.QnAMakerRessources.NoMatchFoundHandlerMessage;
            await context.PostAsync(message.Text);
        }

        private async Task DefaultMatchHandler(IDialogContext context, string queryString, QnAMakerResult qnAMakerResultObject)
        {
            var messageActivity = context.MakeMessage();
            messageActivity.Text = qnAMakerResultObject.Answers.First().Answer;
            await context.PostAsync(messageActivity.Text);
        }

        private async Task<TransferObjectQnA> NoMatchHandlerWithReturn(IDialogContext context, string queryString)
        {
            TransferObjectQnA transfer = new TransferObjectQnA
            {
                Answer = Ressources.QnAMakerRessources.NoMatchFoundHandlerMessage,
                Question = queryString,
                Score = 0
            };
            return transfer;           
        }

        private async Task<TransferObjectQnA> DefaultMatchHandlerWithReturn(IDialogContext context, string queryString, QnAMakerResult qnAMakerResultObject)
        {
            TransferObjectQnA transfer = new TransferObjectQnA
            {
                Answer = qnAMakerResultObject.Answers.First().Answer,
                Question = queryString,
                Score = qnAMakerResultObject.Answers.First().Score
            };
            return transfer;           
        }
    }
}