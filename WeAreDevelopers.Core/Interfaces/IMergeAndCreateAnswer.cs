using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeAreDevelopers.Core.DTO;

namespace WeAreDevelopers.Core.Interfaces
{
    public interface IMergeAndCreateAnswer
    {
        string DoMergeAndCreateAnswer(TransferObjectLuis processedLuisResult, JObject starwarsAnswer);
        string DoMergeAndCreateAnswer(TransferObjectLuis processedLuisResult, JObject starwarsAnswer, JObject StarwarsAnswer2);
    }
}
