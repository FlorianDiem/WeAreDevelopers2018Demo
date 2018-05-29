using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using WeAreDevelopers.Core.Interfaces;


namespace WeAreDevelopers.Core.Classes
{
    public class StarwarsRequest:IStarwarsRequest
    {
        // Starwars API by Paul Hallett
        public async Task<JObject> StarWarsDataRequest(Uri RequestUri)
        {
            JObject responseJObject = new JObject();
            String responseString = String.Empty;
            HttpResponseMessage response = new HttpResponseMessage();

            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
                  
            response = await client.GetAsync(RequestUri);

            responseString = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                responseString = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(responseString))
                {
                    responseJObject = JObject.Parse(responseString);
                    return responseJObject;
                }
                else
                {
                    return new JObject();
                }
            }
            else
            {
                responseJObject.Add("Status Code", response.StatusCode.ToString());
                responseString = await response.Content.ReadAsStringAsync();
                responseJObject.Add("requestBody", JObject.Parse(responseString));
                return responseJObject;
            }
        }

    }
}