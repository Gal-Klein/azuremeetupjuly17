using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ACSMeetup.Web.API.Models;

namespace ACSMeetup.Web.API.Controllers
{
    [RoutePrefix("bot")]
    public class BotController : Controller
    {

        [Route("interact")]
        public ActionResult SmartInteract(string q, string device)
        {
            Response.AppendHeader("Access-Control-Allow-Origin", "*");

            if (device == null)
                device = "alexa";
            LuisManager luis = new LuisManager();
            LUIS intent = luis.ExtractIntent(q);
            NewsBotManager newsBot = new NewsBotManager();
            
            return new JsonResult
            {
                Data = newsBot.GetNewsLinesByIntent(intent, device),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                ContentType = "application/json"
            };
        }
    }
}