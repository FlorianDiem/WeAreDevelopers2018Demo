using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WeAreDevelopers.Luis.Interfaces;
using Microsoft.Bot.Builder.Dialogs;
using WeAreDevelopers.Core.DTO;

namespace WeAreDevelopers.Luis.Classes
{
    public class LuisProcessing : ILuisProcessing
    {
        public async Task<TransferObjectLuis> DoProcessingComplex(IDialogContext context, JObject luisResult)
        {
            string topScoring = luisResult.Value<JObject>("topScoringIntent").Value<string>("intent");
            double score = luisResult.Value<JObject>("topScoringIntent").Value<double>("score");

            if (topScoring == "None" || score < 0.4)
            {
                await context.PostAsync("I am sorry I could not understand your request.");

                return new TransferObjectLuis()
                {
                    Intent = "",
                    Name_0 = "",
                    Name_1 = "",
                    Score = 0
                };
            }
            else
            {
                string[] Split = topScoring.Split('.');

                if (Split[0] == "single") return await DoProcessing(context, luisResult);

                JArray Entities = luisResult.Value<JArray>("entities");

                TransferObjectLuis Transfer = new TransferObjectLuis
                {
                    Intent = Split[1],
                    Score = score,
                    Type = Split[0]
                };

                if (Entities.HasValues == false)
                {
                    await context.PostAsync("I am sorry I could not understand whos " + Split[1] + " your looking for.");
                    return Transfer;
                }

                Transfer = LookForNames(Transfer, Entities);

                return Transfer;
            }

          
        }

        private TransferObjectLuis LookForNames(TransferObjectLuis transfer, JArray entities)
        {
            List<string> names = new List<string>();
            foreach (JObject parsedObject in entities.Children<JObject>())
            {
                foreach (JProperty parsedProperty in parsedObject.Properties())
                {
                    string propertyName = parsedProperty.Name;
                    if ((string)parsedProperty.Value == "Name")
                    {
                        names.Add(parsedObject.Value<string>("entity"));                        
                    }
                }
            }

            if (names.Count == 2)
            {
                transfer.Name_0 = names[0];
                transfer.Name_1 = names[1];
                return transfer;
            }
            else if (names.Count == 1)
            {
                transfer.Name_0 = names[0];
                transfer.Name_1 = "";
                return transfer;
            }
            else
            {
                transfer.Name_0 = "";
                transfer.Name_1 = "";
                return transfer;
            }
        }

        public async Task<TransferObjectLuis> DoProcessing(IDialogContext context, JObject luisResult)
        {
            string topScoring = luisResult.Value<JObject>("topScoringIntent").Value<string>("intent");
            double score = luisResult.Value<JObject>("topScoringIntent").Value<double>("score");
            if (topScoring == "None" || score < 0.4)
            {
                await context.PostAsync("I am sorry I could not understand your request.");

                return new TransferObjectLuis()
                {
                    Intent = "",
                    Name_0 = "",
                    Name_1 = ""
                };
            }
            else
            {
                string[] Split = luisResult.Value<JObject>("topScoringIntent").Value<string>("intent").Split('.');
                JArray Entities = luisResult.Value<JArray>("entities");

                if (Entities.HasValues == false)
                {
                    await context.PostAsync("I am sorry I could not understand whos " + Split[1] + " your looking for.");
                    return new TransferObjectLuis()
                    {
                        Type = "single",
                        Intent = Split[1],
                        Name_0 = "",
                        Name_1 = ""
                    };
                }

                string Name = getName(Entities);

                return new TransferObjectLuis()
                {
                    Intent = Split[1],
                    Name_0 = Name,
                    Name_1 = ""
                };
            }
        }

        private string getName(JArray entities)
        {
            var name = (string)entities.Children().Single(p => (string)p["type"] == "Name")["entity"];
            return name;
        }
    }
}