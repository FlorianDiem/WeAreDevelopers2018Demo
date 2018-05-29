using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeAreDevelopers.Core.DTO;

namespace WeAreDevelopers.Luis.Interfaces
{
    public interface ILuisProcessing
    {
       Task<TransferObjectLuis> DoProcessingComplex(IDialogContext context, JObject luisResult);
       Task<TransferObjectLuis> DoProcessing(IDialogContext context, JObject luisResult);
    }
}
