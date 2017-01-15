using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Bot_Application1
{
    [Serializable]
    public class BotDialog : IDialog <DialogParam>
    {
        DialogParam dp = new DialogParam();

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            var repl = Reply(message.Text);
            if (repl.Length > 2)
            {
                await context.PostAsync(repl);
                context.Wait(MessageReceivedAsync);
            }
            else
                context.Wait(MessageReceivedAsync);
        }

        private string Reply(string msg)
        {
            string a = msg.ToLower();
            if (a.Contains("!help"))
            {
                return "Command '!email X@X.X'  -> change e-mail addres. \nCommand '!delete' -> deleting history of messages. \nCommand '/email [message]' -> sending message to e-mail";
            }
            if (a.Contains("!delete"))
            {
                dp.Del();
                dp.block = 0;
                return @"History has deleted!";
            }
            if (a.Contains("!email") == true)
            {
                string[] mas = a.Split(' ');
                if (IsValid(mas[1]) == true)
                {
                    dp.Set_action(4, mas[1]);
                    dp.block = 0;
                    return dp.BuildResult();
                }
            }
            if (a.Contains("/email") || dp.block == 1)
            {
                dp.block = 1;
                a = a.Replace("/email", "");
                bool res = IsValid(a);
                if (res == true) //введен е-меил
                    dp.Set_action(1, a);
                if (res == false) //введен не е-меил
                {
                    if ((dp.action == 3) && (a.Equals("yes")))
                    {
                        dp.Set_action(1, a);
                        dp.block = 0;
                        return dp.BuildResult();
                    }
                    if ((dp.action == 3) && (a.Equals("no")))
                    {
                        dp.Set_action(0, "empty string");
                        dp.block = 0;
                        return @"To change the email enter command '!email X@X.X'";
                    }
                    else
                    {
                        dp.Set_action(2, a);
                    }
                }
                return dp.BuildResult();
            }
            return "";
        }

        public static bool IsValid(string emailString)
        {
            return Regex.IsMatch(emailString, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }
    }
}