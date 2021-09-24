using MvcShop.Models.Data;
using MvcShop.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcShop.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            List<CategoryVM> categoryVmList;

            using(Db db = new Db())
            {
                categoryVmList = db.Categories.ToArray().OrderBy(X => X.Sorting).Select(x => new CategoryVM(x)).ToList();
            }

            return PartialView("_CategoryMenuPartial", categoryVmList);
        }
        // GET: Shop/Category/name
        public ActionResult Category(string name)
        {
            List<ProductVM> productVmList;

            using(Db db = new Db())
            {
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Desc == name).FirstOrDefault();

                int catId = categoryDTO.Id;

                productVmList = db.Products.ToArray().Where(x => x.CategoryId == catId).Select(x => new ProductVM(x)).ToList();

                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();

                if(productCat == null)
                {
                    var catName = db.Categories.Where(x => x.Desc == name).Select(x => x.Name).FirstOrDefault();
                    ViewBag.CategoryName = catName;
                }
                else
                {
                    ViewBag.CategoryName = productCat.CategoryName;
                }
            }

            return View(productVmList);
        }
        // GET: Shop/product-datails/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            ProductDTO dto;
            ProductVM model;

            int id;

            using(Db db = new Db())
            {
                if(!db.Products.Any(x => x.shortDesc.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }

                dto = db.Products.Where(x => x.shortDesc == name).FirstOrDefault();

                id = dto.Id;

                model = new ProductVM(dto);
            }

            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            return View("ProductDetails", model);
        }
    }
}