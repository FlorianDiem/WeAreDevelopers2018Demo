﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Linq;
using WeAreDevelopers.Core.Interfaces;
using WeAreDevelopers.Core.Classes;
using Newtonsoft.Json.Linq;

namespace WeAreDevelopersBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () =>
                new Dialogs.CallLuisCallStarwarsAPIorQnADialog());
            }
            else
            {
                await HandleSystemMessageAsync(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task HandleSystemMessageAsync(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels

                if (message.MembersAdded.Any(o => o.Id == message.Recipient.Id))
                {
                    /* ConnectorClient client = new ConnectorClient(new Uri(message.ServiceUrl));
                     * var reply = message.CreateReply();
                     * reply.Text = "Welcome to the Bot to showcase the DirectLine API. Send 'Show me a hero card' or 'Send me a BotFramework image' to see how the DirectLine client supports custom channel data. Any other message will be echoed.";
                     * await client.Conversations.ReplyToActivityAsync(reply);
                     */
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }
        }
    }
}