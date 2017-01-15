using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Microsoft.Bot.Builder.Dialogs;
using System.Collections.Generic;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Emotion;

namespace Bot_Application1
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
            if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
            {
                try
                {
                    ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                    String res = await Emotion(activity);
                    Activity reply = activity.CreateReply(res);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    return response;
                }
                catch (Exception e)
                {
                    await Conversation.SendAsync(activity, () => new BotDialog());
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        private Activity HandleSystemMessage(Activity message)
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
            return null;
        }

        private async Task<string> Emotion(Activity activity)
        {
            Emotion[] emotionResult;
            float[] em_score = new float[8];
            float max;
            Int32 index = 0;
            EmotionServiceClient emotionServiceClient = new EmotionServiceClient("e1f3a28925ef48cbb032dfa7e8b31be7");
            emotionResult = await emotionServiceClient.RecognizeAsync(activity.Attachments[0].ContentUrl);
            em_score[0] = emotionResult[0].Scores.Anger;
            em_score[1] = emotionResult[0].Scores.Contempt;
            em_score[2] = emotionResult[0].Scores.Disgust;
            em_score[3] = emotionResult[0].Scores.Fear;
            em_score[4] = emotionResult[0].Scores.Happiness;
            em_score[5] = emotionResult[0].Scores.Neutral;
            em_score[6] = emotionResult[0].Scores.Sadness;
            em_score[7] = emotionResult[0].Scores.Surprise;
            max = em_score[0];
            for (Int32 i = 0; i < 8; i++)
            {
                if (em_score[i] > max)
                {
                    max = em_score[i];
                    index = i;
                }
            }
            String res = Recognize_emotion(index);
            return res;
        }

        public String Recognize_emotion(Int32 i)
        {
            String res;
            switch (i)
            {
                case 0: //anger - гнев
                    res = "(angry)";
                    break;
                case 1: //contempt - презрение
                    res = "(smirk)";
                    break;
                case 2: //disgust - отвращение
                    res = "(disgust)";
                    break;
                case 3: //fear - страх
                    res = "(fear)";
                    break;
                case 4: //happiness - счастье
                    res = "(happy)";
                    break;
                case 5: //neutral - нейтральный
                    res = ":|";
                    break;
                case 6: //sadness - печаль
                    res = "(sadness)";
                    break;
                case 7: //surprise - удивление
                    res = ":o";
                    break;
                default:
                    res = "(tmi)";
                    break;
            }
            return res;
        }
    }
}