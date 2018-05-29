using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeAreDevelopers.Luis.Interfaces
{
    public interface ILuisRequest
    {
        Task<JObject> MakeLuisRequest(string QueryString, string AppId, string subscriptionKey);
    }
}
