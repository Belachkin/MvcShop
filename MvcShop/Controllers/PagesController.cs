using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcShop.Controllers
{
    public class PagesController : Controller
    {
        // GET: Pages
        public ActionResult Index()
        {
            return View();
        }
    }
}