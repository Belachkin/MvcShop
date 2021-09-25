using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcShop.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        //GET: Account/Registration
        [ActionName("Registration")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        //TODO: Добавить вывод инфы об аккаунте
        //TODO: Добавить редактор аккаунта
        //TODO: Добавить возможность удалить аккунт с капчей
    }
}