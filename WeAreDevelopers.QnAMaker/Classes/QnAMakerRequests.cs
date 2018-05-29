using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using WeAreDevelopers.QnAMaker.Interfaces;

namespace WeAreDevelopers.QnAMaker.Classes
{
    public class QnAMakerRequests:IQnAMakerRequests
    {        
        public async Task<QnAMakerResult> GetQnAMakerResponse(string query, string knowledgeBaseId, string subscriptionKey)
        {
            string responseString = string.Empty;

            var knowledgebaseId = knowledgeBaseId;          // Use knowledge base id created.
            var qnamakerSubscriptionKey = subscriptionKey;  //Use subscription key assigned to you.

            //Build the URI
            Uri qnamakerUriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v2.0");
            var builder = new UriBuilder($"{qnamakerUriBase}/knowledgebases/{knowledgebaseId}/generateAnswer");

            //Add the question as part of the body
            var postBody = $"{{\"question\": \"{query}\"}}";

            //Send the POST request
            using (WebClient client = new WebClient())
            {
                //Set the encoding to UTF8
                client.Encoding = System.Text.Encoding.UTF8;

                //Add the subscription key header
                client.Headers.Add("Ocp-Apim-Subscription-Key", qnamakerSubscriptionKey);
                client.Headers.Add("Content-Type", "application/json");
                responseString = client.UploadString(builder.Uri, postBody);
            }

            //De-serialize the response
            QnAMakerResult response;
            try
            {
               response = JsonConvert.DeserializeObject<QnAMakerResult>(responseString);
               return response;
            }
            catch
            {
                throw new Exception("Something went wrong during deserialization.");
            }
        }
    }
}