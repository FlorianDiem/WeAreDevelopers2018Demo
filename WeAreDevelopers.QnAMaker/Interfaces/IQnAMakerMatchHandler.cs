using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeAreDevelopers.Core.DTO;
using WeAreDevelopers.QnAMaker.Classes;

namespace WeAreDevelopers.QnAMaker.Interfaces
{
    public interface IQnAMakerMatchHandler
    {
        Task<TransferObjectQnA> QnAMakerResultProcessingWithReturn(IDialogContext context, string QueryString, QnAMakerResult QnAMakerResultObject);
        Task QnAMakerResultProcessing(IDialogContext context, string QueryString, QnAMakerResult QnAMakerResultObject);
    }
}
