using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WeAreDevelopers.Core.Classes;
using WeAreDevelopers.Core.Interfaces;
namespace WeAreDevelopersBot.Dialogs
{
    [Serializable]
    public class NumbersDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // to do erros abfangen 
            await CallStarWarsApi(context,activity.Text);
            
            context.Wait(MessageReceivedAsync);
        }

        private async Task CallStarWarsApi(IDialogContext context,string UserQuestion)
        {
            Uri BaseUri = new Uri("https://swapi.co/api/" + UserQuestion);
            IStarwarsRequest starwars = new StarwarsRequest();
            JObject Dataset = await starwars.StarWarsDataRequest(BaseUri);

            await context.PostAsync(Dataset.ToString());
        }
               
        private async Task PostErrorMessage(IDialogContext context)
        {
            await context.PostAsync("Error 1");
        }
    }
}