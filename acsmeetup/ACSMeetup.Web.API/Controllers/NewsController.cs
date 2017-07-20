using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ACSMeetup.Web.API.Models;

namespace ACSMeetup.Web.API.Controllers
{
    [RoutePrefix("news")]
    public class NewsController : Controller
    {
        [Route("headers/{category}")]
        public ActionResult Headers(string category)
        {
            Response.AppendHeader("Access-Control-Allow-Origin", "*");
            WebNewsReader reader = new WebNewsReader();
            var results = reader.GetTitles(category, 5);
            return new JsonResult { Data = results, JsonRequestBehavior = JsonRequestBehavior.AllowGet, ContentType = "application/json" };
        }

        [Route("ask/{question}")]
        public ActionResult Ask(string question)
        {
            Response.AppendHeader("Access-Control-Allow-Origin", "*");
            WebNewsReader reader = new WebNewsReader();
            var results = reader.SearchTitle(question);
            return new JsonResult { Data = results, JsonRequestBehavior = JsonRequestBehavior.AllowGet, ContentType = "application/json" };
        }
    }
}