using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PostHost.Models;
using Microsoft.AspNet.Identity;

namespace PostHost.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult testAdding()
        {
            return View();
        }
        [HttpPost]
        public ActionResult testAdding(Content toAdd)
        {
            var user = User.Identity.GetUserId();
            
            toAdd.PostedBy = user;


            using (PostHostDBEntities phdbec = new PostHostDBEntities())
            {
                phdbec.Contents.Add(toAdd);
                phdbec.SaveChanges();
            }
            return View();
        }
    }
}