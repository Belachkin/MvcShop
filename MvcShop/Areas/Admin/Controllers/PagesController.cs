using MvcShop.Models.Data;
using MvcShop.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcShop.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PagesController : Controller
    {
        // GET: Admin/Pages/Index
        public ActionResult Index()
        {
            List<PageVM> pageList;
            using (Db db = new Db())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();                
            }
            return View(pageList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //Проверка на валидность
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            //Используем БД
            using(Db db = new Db())
            {

                string sDesc;
                PagesDTO dto = new PagesDTO();
                dto.Title = model.Title.ToUpper();
                //Проверка есть ли описание, если нет, то присваиваем и заменяем пробелы на тире, и так же делаем текст маленьким
                if(string.IsNullOrWhiteSpace(model.shortDesc))
                {
                    sDesc = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    sDesc = model.shortDesc.Replace(" ", "-").ToLower();
                }
                //Проверка на уникальность
                if(db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("","That title already exist.");
                    return View(model);
                }
                else if(db.Pages.Any(x => x.shortDesc == model.shortDesc))
                {
                    ModelState.AddModelError("", "That shortDesc already exist.");
                }

                dto.shortDesc = sDesc;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                db.Pages.Add(dto);
                db.SaveChanges();
            }

            TempData["SM"] = "You have added a new page.";

            return RedirectToAction("Index");
            
        }

        // GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            PageVM model;

            using (Db db = new Db())
            {
                //Получаем данные по id
                PagesDTO dto = db.Pages.Find(id);
                //Проверяем доступность
                if(dto == null)
                {
                    return Content("The page does not exist.");
                }
                //Присваиваем модели получиные данные из бд
                model = new PageVM(dto);

            }
            return View(model);
        }

        // POST: Admin/Pages/EditPage
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //Проверка валидности
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            using(Db db = new Db())
            {
                int id = model.Id;
                string sDesc = null;
                //Получаем модель по id
                PagesDTO dto = db.Pages.Find(id);

                dto.Title = model.Title;
                if(model.shortDesc != null)
                {
                    if(string.IsNullOrWhiteSpace(model.shortDesc))
                    {
                        sDesc = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        sDesc = model.shortDesc.Replace(" ", "-").ToLower();
                    }
                }
                

                //Проверка на совподения в других страницах
                if(db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title alredy exist.");
                }
                else if(db.Pages.Where(x => x.Id != id).Any(x => x.shortDesc == sDesc))
                {
                    ModelState.AddModelError("", "That shortDesc alredy exist.");
                }

                dto.shortDesc = sDesc;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                db.SaveChanges();

            }

            TempData["SM"] = "You Have edited the page.";

            return RedirectToAction("EditPage");
        }
        // GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            PageVM model;

            using(Db db = new Db())
            {
                PagesDTO dto = db.Pages.Find(id);

                if(dto == null)
                {
                    return Content("The page does not exist.");
                }

                model = new PageVM(dto);
            }

            return View(model);
        }
        // GET: Admin/Pages/DeletePage/id
        [HttpGet]
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                PagesDTO dto = db.Pages.Find(id);
                
                db.Pages.Remove(dto);

                db.SaveChanges();

                
            }

            TempData["SM"] = "You have deleted a page.";

            return RedirectToAction("Index");
        }
        // Post: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using(Db db = new Db())
            {
            //начальный счетчик
                int count = 1;

                PagesDTO dto;
            //устанавливаем сортировку
            foreach(var pageId in id)
            {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
            }

            }
            
        }
        // GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            SidebarVM model;

            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebars.Find(1); //TODO: Пофиксить гавнокод

                model = new SidebarVM(dto);
            }
            return View(model);
        }
        // POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using(Db db = new Db())
            {
                SidebarDTO dto = db.Sidebars.Find(1);//TODO: Пофиксить гавнокод 2

                dto.Body = model.Body;

                db.SaveChanges();
            }

            TempData["SM"] = "You have edited the sidebar.";
            return RedirectToAction("EditSidebar");
        }

    }
}