using MvcShop.Models.Data;
using MvcShop.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MvcShop.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        //GET: Account/Registration
        [ActionName("Registration")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        [ActionName("Registration")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            //Проверка валидности
            if(!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }
            //Проверка соответствия паролей
            if(!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Password do not match!");
                return View("CreateAccount", model);
            }
            using(Db db = new Db())
            {
                //Проверка уникальности
                if(db.Users.Any(x => x.Username.Equals(model.Username)))
                {
                    ModelState.AddModelError("", $"Username: {model.Username} is taken");
                    model.Username = "";
                    return View("CreateAccount", model);
                }
                if (db.Users.Any(x => x.Password.Equals(model.Password)))
                {
                    ModelState.AddModelError("", $"Passwor is taken");
                    model.Password = "";
                    model.ConfirmPassword = "";
                    return View("CreateAccount", model);
                }
                if (db.Users.Any(x => x.EmailAdress.Equals(model.EmailAdress)))
                {
                    ModelState.AddModelError("", $"Email {model.EmailAdress} is taken");
                    model.EmailAdress = "";                    
                    return View("CreateAccount", model);
                }

                UserDTO userDto = new UserDTO()
                {
                    FirstName   = model.FirstName,
                    LastName    = model.LastName,
                    EmailAdress = model.EmailAdress,
                    Username    = model.Username,
                    Password    = model.Password
                };

                db.Users.Add(userDto);
                db.SaveChanges();

                //Названчение роли
                int id = userDto.Id;

                UserRoleDTO userRoleDTO = new UserRoleDTO() 
                {
                    UserId = id,
                    RoleId = 2 //Гавнокод :c
                };

                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
            }

            TempData["SM"] = "You are now registered and can login";

            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Login()
        {
            //Проверка авторизации
            string username = User.Identity.Name;

            if(!string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Profile");
            }

            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            //Проверка валидности модели
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            //Проверка валидности пользователя

            bool isValid = false;

            using(Db db = new Db())
            {
                if(db.Users.Any(x => x.Username.Equals(model.Username) && x.Password.Equals(model.Password)))
                {
                    isValid = true;
                }

                if(!isValid)
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View(model);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
                }
            }           
        }
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        //TODO: Добавить вывод инфы об аккаунте
        //TODO: Добавить редактор аккаунта
        //TODO: Добавить возможность удалить аккунт с капчей
    }
}