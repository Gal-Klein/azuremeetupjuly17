using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACSMeetup.Web.API.Models
{
    public static class Settings
    {
        public static string HelpMessage = "This is a Meetup app, do not expect too much. Just ask: What's the lates on a news subject, or read me the us news";
        public static string WelcomeMessage = "I am The Amazing Meetup AI, I know what is in the news.";
        public static string NotSureMessage = "Sorry, I am unable to understand what it is you are searching for.";
        public static string UghMessage = "Sorry, No idea what you want from me.";

        public static string NoResultsFoundMessage(string term)
        {
            return $"Sorry, I am unable to find any news story on {term}.";
        }
        public static string ResultsFoundMessage(string term)
        {
            return $"Here is what I found On: {term}.";
        }
        public static string TopResultsFoundMessage(string term)
        {
            return $"Here are the top {term} news.";
        }
    }
}