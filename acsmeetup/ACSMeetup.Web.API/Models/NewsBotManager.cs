using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACSMeetup.Web.API.Models
{
    public class NewsBotManager
    {
        public NewsBotManager()
        {

        }

        public NewsBotResponse GetNewsLinesByIntent(LUIS intent, string device) {
            WebNewsReader reader = new WebNewsReader();
            switch (intent.topScoringIntent.intent.ToLower())
            {
                case "help":
                    return new NewsBotResponse
                    {
                        action = "Help",
                        SSML = MessageToSSML.Convert(Settings.HelpMessage, device),
                        text = Settings.HelpMessage
                    };
                case "welcome":
                    return new NewsBotResponse
                    {
                        action = "Welcome",
                        SSML = MessageToSSML.Convert(Settings.WelcomeMessage, device),
                        text = Settings.WelcomeMessage
                    };
                case "question":
                    if (intent.entities.Length == 0)
                    {
                        return new NewsBotResponse
                        {
                            action = "Question",
                            SSML = MessageToSSML.Convert(Settings.NotSureMessage, device),
                            text = Settings.NotSureMessage
                        };
                    }
                    var result = reader.SearchTitle(intent.entities[0].entity);
                    if (result == null)
                    {
                        return new NewsBotResponse
                        {
                            action = "Question",
                            SSML = MessageToSSML.Convert(Settings.NoResultsFoundMessage(intent.entities[0].entity), device),
                            text = Settings.NoResultsFoundMessage(intent.entities[0].entity)
                        };
                    }
                    string qtext = $"{Settings.ResultsFoundMessage(intent.entities[0].entity)}, {result.title}";
                    List<string> list = new List<string>();
                    list.Add(Settings.ResultsFoundMessage(intent.entities[0].entity));
                    list.Add("0.5");
                    list.Add(result.title);
                    return new NewsBotResponse
                    {
                        action = "Question",
                        SSML = MessageToSSML.Convert(list.ToArray(), device),
                        text = qtext
                    };
                case "readnews":
                    var results = reader.GetTitles(intent.entities[0].entity, 5);
                    List<string> nlist = new List<string>();
                    string ntext = Settings.TopResultsFoundMessage(intent.entities[0].entity);
                    nlist.Add(Settings.TopResultsFoundMessage(intent.entities[0].entity));
                    nlist.Add("0.5");
                    foreach (var r in results)
                    {
                        nlist.Add("ding.mp3");
                        ntext += $"From {r.source}, {r.title} ";
                        nlist.Add($"From {r.source}, ");
                        nlist.Add("0.5");
                        nlist.Add(r.title + " ");
                        nlist.Add("1");
                    }
                    return new NewsBotResponse
                    {
                        action = "ReadNews",
                        SSML = MessageToSSML.Convert(nlist.ToArray(), device),
                        text = ntext
                    };
                    #region Breaking News
                    case "breaking":
                        var bresults = reader.GetTitles("breaking", 5);
                        List<string> blist = new List<string>();
                        string btext = Settings.TopResultsFoundMessage("Breaking News");
                        blist.Add(Settings.TopResultsFoundMessage("Breaking News"));
                        blist.Add("0.5");
                        foreach (var r in bresults)
                        {
                            btext += $"From AP News, {r.title} ";
                            blist.Add($"From {r.source}, ");
                            blist.Add("0.5");
                            blist.Add(r.title + " ");
                            blist.Add("1");
                        }
                        return new NewsBotResponse
                        {
                            action = "ReadNews",
                            SSML = MessageToSSML.Convert(blist.ToArray(), device),
                            text = btext
                        };
                    #endregion
            }
            return new NewsBotResponse
            {
                action = "NoIdea",
                SSML = MessageToSSML.Convert(Settings.UghMessage, device)
            };
        } 
    }

    public class NewsBotResponse
    {
        public string action { get; set; }
        public string SSML { get; set; }
        public string text { get; set; }
    }
}