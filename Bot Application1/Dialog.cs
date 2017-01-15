using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace Bot_Application1
{
    [Serializable]
    public class DialogParam
    {
        public int block = 0;
        public int action;
        public string message { get; set; }
        public string mail { get; set; }
        public DialogParam()
        {
            message = "empty history";
            action = 0;
            mail = "";
        }
        public void Set_action(int a, string m)
        {
            action = a;
            if (action == 1)
            {
                if (mail.Length == 0)
                    mail = m;
            }
            if (action == 2)
                message = m;
            if (action == 4)
                mail = m;
        }
        public void Del()
        {
            message = "empty history";
            action = 0;
            mail = "";
        }
        public string BuildResult()
        {
            StringBuilder sb = new StringBuilder();
            if (action == 1)
            {
                if (SendMail(message, mail) == true)
                {
                    sb.Append($"Message ( {message} ) was sended!");
                    action = 3;
                    block = 0;
                }
                else
                    action = 2;
            }
            if (action == 2)
            {
                if (mail.Length == 0)
                    sb.Append($"Enter e-mail for sending message ( {message} ): ");
                else
                {
                    action = 3;
                    sb.Append($"Should I send ( {message} ) to e-mail -> {mail} ? (yes / no)");
                }
            }
            if (action == 4)
                sb.Append($"The email was changed!");
            if (sb.Length == 0)
                return "I don't understand";
            else
                return sb.ToString();
        }
        public bool SendMail(string text, string to)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.Host = "smtp.gmail.com";
                //client.UseDefaultCredentials = false;
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("sashutkabrest@gmail.com", "19x01v96u");
                MailMessage Message = new MailMessage("sashutkabrest@gmail.com", to, "Message from bot", text);
                client.Send(Message);
                return true;
            }
            catch (Exception e)
            {
                message = e.ToString();
                return false;
            }
        }
    }
}