using MvcShop.Models.Data;
using MvcShop.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcShop.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{page}
        public ActionResult Index(string page = "")
        {
            if(page == "") 
                { page = "home"; }

            PageVM model;
            PagesDTO dto;

            using(Db db = new Db())
            {
                if(!db.Pages.Any(x => x.shortDesc.Equals(page))) //если не найдена страница то переадресация на пустую
                {
                    return RedirectToAction("Index", new { page = "" });
                }
                else
                {
                    dto = db.Pages.Where(x => x.shortDesc == page).FirstOrDefault();
                }                
            }

            ViewBag.PageTitle = dto.Title;

            //Имеется ли sidebar
            if (dto.HasSidebar == true) // TODO: пофиксить гавнокод
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }

            //Заполнить модель
            model = new PageVM(dto);
                

            return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            List<PageVM> pageVmList;

            using(Db db = new Db())
            {
                pageVmList = db.Pages.ToArray()
                    .OrderBy(x => x.Sorting)
                    .Where(x => x.shortDesc != "home")
                    .Select(x => new PageVM(x))
                    .ToList();
            }

            return PartialView("_PagesMenuPartial", pageVmList);
        }

        public ActionResult SidebarPartial()
        {
            SidebarVM model;

            using(Db db = new Db())
            {
                SidebarDTO dto = db.Sidebars.Find(1);

                model = new SidebarVM(dto);
            }

            return PartialView("_SidebarPartial",model);
        }
    }
}