using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.ServiceModel.Syndication;

namespace ACSMeetup.Web.API.Models
{
    public class WebNewsReader
    {
        public WebNewsReader()
        {

        }

        public ArticleItem SearchTitle (string term)
        {
            string[] categoriesToSerach = { "us", "world", "business", "sport" };
            foreach( var c in categoriesToSerach)
            {
                ArticleItem[] items = GetTitles(c, 10);
                ArticleItem item = (from i in items where i.title.ToLower().Contains(term.ToLower()) select i).FirstOrDefault();
                if (item != null)
                    return item;
            }
            return null;
        }

        public ArticleItem[] GetTitles(string category, int count)
        {
            RSSTitles rss = GetRssPerTopic(category);
            List<ArticleItem> list = new List<ArticleItem>();
            foreach (var r in rss.RSSFeeds)
            {
                XmlReader reader = XmlReader.Create(r);
                SyndicationFeed rssFeed = SyndicationFeed.Load(reader);
                reader.Close();
                var lastTwentyItems = rssFeed.Items.Take(count);
                foreach (SyndicationItem item in lastTwentyItems)
                {
                    string title = item.Title.Text;
                    string[] arr = title.Replace(" - ", "|").Split('|');
                    list.Add(new ArticleItem
                    {
                        category = rss.TopicId,
                        title = arr[0],
                        source = arr.Length > 1 ? arr[1] : "Web"
                    });
                }
            }
            return list.ToArray();
        }

        private RSSTitles GetRssPerTopic(string category)
        {
            List<string> urls = new List<string>();
            string topic = "";
            switch (category.ToLower())
            {
                case "business":
                    topic = "Business";
                    urls.Add("https://news.google.com/news?cf=all&hl=en&pz=1&ned=us&topic=b&output=rss");
                    urls.Add("https://www.bing.com/news/search?q=&nvaug=%5bNewsVertical+Category%3d%22rt_Business%22%5d&FORM=NSBABR&format=rss");
                    break;
                case "us":
                case "news":
                case "us news":
                    topic = "US News";
                    urls.Add("https://news.google.com/news?cf=all&hl=en&pz=1&ned=us&topic=n&output=rss");
                    urls.Add("https://www.bing.com/news/search?q=&nvaug=%5bNewsVertical+Category%3d%22rt_Politics%22%5d&FORM=NSBABR&format=rss");
                    break;
                case "tech":
                case "technology":
                    topic = "Tech";
                    urls.Add("https://news.google.com/news?cf=all&hl=en&pz=1&ned=us&topic=tc&output=rss");
                    urls.Add("https://www.bing.com/news/search?q=&nvaug=%5bNewsVertical+Category%3d%22rt_ScienceAndTechnology%22%5d&FORM=NSBABR&format=rss");
                    break;
                case "world":
                case "foreign":
                    topic = "World";
                    urls.Add("https://news.google.com/news?cf=all&hl=en&pz=1&ned=us&topic=w&output=rss");
                    urls.Add("https://www.bing.com/news/search?q=&nvaug=%5bNewsVertical+Category%3d%22rt_World%22%5d&FORM=NSBABR&format=rss");
                    break;
                case "science":
                    topic = "Science";
                    urls.Add("https://news.google.com/news?cf=all&hl=en&ned=us&topic=snc&output=rss");
                    break;
                case "health":
                    topic = "Health";
                    urls.Add("https://news.google.com/news?cf=all&hl=en&pz=1&ned=us&topic=m&output=rss");
                    urls.Add("https://www.bing.com/news/search?q=&nvaug=%5bNewsVertical+Category%3d%22rt_Health%22%5d&FORM=NSBABR&format=rss");
                    break;
                case "entertainment":
                    topic = "Entertainment";
                    urls.Add("https://news.google.com/news?cf=all&hl=en&pz=1&ned=us&topic=e&output=rss");
                    urls.Add("https://www.bing.com/news/search?q=&nvaug=%5bNewsVertical+Category%3d%22rt_Entertainment%22%5d&FORM=NSBABR&format=rss");
                    break;
                case "sport":
                case "sports":
                    topic = "Sports";
                    urls.Add("https://news.google.com/news?cf=all&hl=en&pz=1&ned=us&topic=s&output=rss");
                    urls.Add("https://www.bing.com/news/search?q=&nvaug=%5bNewsVertical+Category%3d%22rt_Sports%22%5d&FORM=NSBABR&format=rss");
                    break;
                case "breaking":
                    topic = "Breaking News";
                    urls.Add("http://hosted2.ap.org/atom/APDEFAULT/3d281c11a96b4ad082fe88aa0db04305");
                    break;
            }
            return new RSSTitles
            {
                TopicId = topic,
                RSSFeeds = urls.ToArray()
            };
        }
    }

    public class RSSTitles
    {
        public string TopicId { get; set; }
        public string[] RSSFeeds { get; set; }
    }

    public class ArticleItem
    {
        public string title { get; set; }
        public string source { get; set; }
        public string category { get; set; }
    }
}