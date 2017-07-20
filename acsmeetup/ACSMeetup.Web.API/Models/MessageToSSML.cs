using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACSMeetup.Web.API.Models
{
    public static class MessageToSSML
    {
        static string contentStorageURL = "<Enter Your Storage URL Where MP3 files reside>";
        public static string Convert(string[] entries, string device)
        {
            string message = "<speak>";
            if (device == null)
                device = "alexa";
            if (device.ToLower() == "cortana")
                message = "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\">";
            foreach(var e in entries)
            {
                double breakTime = 0;
                if(double.TryParse(e, out breakTime))
                {
                    message += $"<p><break time=\"{breakTime}s\"/></p>";
                }
                else
                {
                    if (e.EndsWith(".mp3"))
                        message += $"<audio src='{contentStorageURL}/{e}'></audio>";
                    else
                        message += $"<p>{e}</p>";
                }
            }
            message += "</speak>";
            return message;
        }

        public static string Convert(string text, string device)
        {
            string message = "<speak>";
            if (device == null)
                device = "alexa";
            if (device.ToLower() == "cortana")
                message = "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\">";
            message += $"<p>{text}</p>";
            message += "</speak>";
            return message;
        }
    }
}