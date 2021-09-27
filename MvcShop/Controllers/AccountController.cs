using MvcShop.Models.Data;
using MvcShop.Models.ViewModels.Account;
using MvcShop.Models.ViewModels.Orders;
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
        [Authorize]
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        public ActionResult UserNavPartial()
        {
            string userName = User.Identity.Name;

            UserNavPartialVM model;

            using(Db db = new Db())
            {
                UserDTO userDTO = db.Users.FirstOrDefault(x => x.Username == userName);

                model = new UserNavPartialVM()
                {
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName
                };
            }

            return PartialView("_UserNavPartial",model);

        }
        [ActionName("Profile")]
        [HttpGet]
        [Authorize]
        public ActionResult UserProfile()
        {
            string userName = User.Identity.Name;

            UserProfileVM model;

            using(Db db = new Db())
            {
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);

                model = new UserProfileVM(dto);
            }

            return View("UserProfile", model);
        }
        [ActionName("Profile")]
        [HttpPost]
        [Authorize]
        public ActionResult UserProfile(UserProfileVM model)
        {
            bool userNameIsChanged = false;

            if(!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                if(!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Password do not match");
                    return View("UserProfile", model);
                }
            }

            using(Db db = new Db())
            {
                string userName = User.Identity.Name;

                if(userName != model.Username)
                {
                    userName = model.Username;
                    userNameIsChanged = true;
                }

                if(db.Users.Where(x => x.Id != model.Id).Any(x => x.Username == userName))
                {
                    ModelState.AddModelError("", $"Username: {model.Username} alredy exists");
                    model.Username = "";
                    return View("UserProfile", model);
                }

                UserDTO dto = db.Users.Find(model.Id);

                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;

                if (!string.IsNullOrWhiteSpace(model.Username))
                {
                    dto.Username = model.Username;
                }

                if (db.Users.Where(x => x.Id != model.Id).Any(x => x.EmailAdress == model.EmailAdress))
                {
                    ModelState.AddModelError("", "Email alredy exists");
                    model.EmailAdress = "";                   
                    return View("UserProfile", model);
                }
                if (!string.IsNullOrWhiteSpace(model.EmailAdress))
                {
                    dto.EmailAdress = model.EmailAdress;
                }

                

                if (db.Users.Where(x => x.Id != model.Id).Any(x => x.Password == model.Password))
                {
                    ModelState.AddModelError("", "Password alredy exists");
                    model.Password = "";
                    model.ConfirmPassword = "";
                    return View("UserProfile", model);
                }
                if(!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }

                db.SaveChanges();

            }

            TempData["SM"] = "You have edited profile";

            if(!userNameIsChanged)
            {
                return View("UserProfile", model);
            }
            else
            {
                return RedirectToAction("Logout");
            }

            
        }
        [Authorize]
        public ActionResult Orders()
        {
            string userName = User.Identity.Name;

            List<OrderVM> orderDTO;

            using (Db db = new Db())
            {
                var user = db.Users.FirstOrDefault(x => x.Username == userName);
                int userId = user.Id;                            

                orderDTO = db.Orders.Where(x => x.UserId == userId).ToArray().Select(x => new OrderVM(x)).ToList();
            }

            return View(orderDTO);
        }

    }
}