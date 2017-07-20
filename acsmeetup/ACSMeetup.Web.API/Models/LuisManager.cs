using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ACSMeetup.Web.API.Models
{
    public class LuisManager
    {
        string luisAPI = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/17f9b30a-6efe-4aa5-9162-7dd67d0548df?subscription-key=2ede382db6f14b9ca00d8ac910aaec90&verbose=true&timezoneOffset=0&q=";
        public LuisManager()
        {

        }

        public LUIS ExtractIntent(string search)
        {
            using (WebClient wc = new WebClient())
            {
                return JsonConvert.DeserializeObject<LUIS>(wc.DownloadString(luisAPI+search));
            }
        }
    }
    public class LUIS
    {
        public string query { get; set; }
        public LUISIntent topScoringIntent { get; set; }
        public LUISIntent[] intents { get; set; }
        public LUISEntity[] entities { get; set; }
    }

    public class LUISIntent
    {
        public string intent { get; set; }
        public double? score { get; set; }
    }

    public class LUISEntity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int? startIndex { get; set; }
        public int? endIndex { get; set; }
        public double? score { get; set; }
        public LUISResolution resolution { get; set; }
    }

    public class LUISResolution
    {
        public string time { get; set; }
        public string date { get; set; }
        public string value { get; set; }
    }
}