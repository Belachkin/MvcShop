using MvcShop.Models.Data;
using MvcShop.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcShop.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        //Category------------------------------------
        // GET: Admin/Shop/Categories
        public ActionResult Categories()
        {
            List<CategoryVM> categoryVmList;

            using(Db db = new Db())
            {
                categoryVmList = db.Categories
                    .ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(x => new CategoryVM(x))
                    .ToList();
            }

            return View(categoryVmList);
        }
        // POST: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            string id;

            using(Db db = new Db())
            {
                //Проверка на уникальность
                if(db.Categories.Any(x => x.Name == catName)) 
                { 
                    return "titletaken"; 
                }

                CategoryDTO dto = new CategoryDTO();
                
                dto.Name = catName;
                dto.Desc = catName.Replace(" ", "-").ToLower();

                db.Categories.Add(dto);
                db.SaveChanges();

                id = dto.Id.ToString();
            }

            return id;
        }

        // Post: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                //начальный счетчик
                int count = 1;

                CategoryDTO dto;
                //устанавливаем сортировку
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }

            }

        }
        // GET: Admin/Shop/DeleteCategory/id
        [HttpGet]
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                CategoryDTO dto = db.Categories.Find(id);

                db.Categories.Remove(dto);

                db.SaveChanges();

            }

            TempData["SM"] = "You have deleted a category.";

            return RedirectToAction("Categories");
        }
        // Post: Admin/Shop/RenameCategory/id
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                //Проверка уникальности
                if(db.Categories.Any(x => x.Name == newCatName))
                {
                    return "titletaken";
                }

                CategoryDTO dto = db.Categories.Find(id);

                dto.Name = newCatName;
                dto.Desc = newCatName.Replace(" ", "-").ToLower();

                db.SaveChanges();
            }

            return "ok";


            
        }

        //Product----------------------------------------
        // GET: Admin/Shop/AddProduct 
        public ActionResult AddProduct()
        {
            ProductVM model = new ProductVM();

            using(Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
            }

            return View(model);
        }

    }
}