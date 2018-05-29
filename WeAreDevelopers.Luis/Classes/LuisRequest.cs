using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using WeAreDevelopers.Luis.Interfaces;

namespace WeAreDevelopers.Luis.Classes
{
    public class LuisRequest:ILuisRequest
    {
        public async Task<JObject> MakeLuisRequest(string QueryString, string AppId, string subscriptionKey)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // The request header contains your subscription key
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // The "q" parameter contains the utterance to send to LUIS
            queryString["q"] = QueryString;

            // These optional request parameters are set to their default values
            queryString["timezoneOffset"] = "0";
            queryString["verbose"] = "false";
            queryString["spellCheck"] = "false";
            queryString["staging"] = "false";

            var uri = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/" + AppId + "?" + queryString;
            var response = await client.GetAsync(uri);

            var strResponseContent = await response.Content.ReadAsStringAsync();
            return JObject.Parse(strResponseContent);
        }
    }
}