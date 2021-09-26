using MvcShop.Models.Data;
using MvcShop.Models.ViewModels.Shop;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using PagedList;

namespace MvcShop.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
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
        [HttpGet]
        public ActionResult AddProduct()
        {
            ProductVM model = new ProductVM();

            using(Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
            }

            return View(model);
        }
        [HttpPost]
        // POST: Admin/Shop/AddProduct 
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            //Проверка валидности
            if(!ModelState.IsValid)
            {
                using(Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }
            //Проверка уникальности
            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "That product is taken.");
                    return View(model);
                }
            }

            int id;

            using(Db db = new Db())
            {
                ProductDTO prodDto = new ProductDTO();

                prodDto.Name = model.Name;
                prodDto.shortDesc = model.Name.Replace(" ", "-").ToLower();
                prodDto.Price = model.Price;
                prodDto.longDesc = model.longDesc;
                prodDto.CategoryId = model.CategoryId;

                CategoryDTO catDto = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                prodDto.CategoryName = model.CategoryName;

                db.Products.Add(prodDto);
                db.SaveChanges();

                id = prodDto.Id;

            }

            TempData["SM"] = "You have added a product.";

            #region Updload Img
            
            // Создаем необходимые ссылки дериктории (Крч шиза)
            var OriginalDir = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            var pathString1 = Path.Combine(OriginalDir.ToString(), "Products");
            var pathString2 = Path.Combine(OriginalDir.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(OriginalDir.ToString(), "Products\\" + id.ToString() + "\\Thumbs" );
            var pathString4 = Path.Combine(OriginalDir.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(OriginalDir.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            // Проверяем наличие дерикторий (если нет то создать) Сори за гавнокод :c
            if (!Directory.Exists(pathString1))
            {
                Directory.CreateDirectory(pathString1);
            }
            if (!Directory.Exists(pathString2))
            {
                Directory.CreateDirectory(pathString2);
            }
            if (!Directory.Exists(pathString3))
            {
                Directory.CreateDirectory(pathString3);
            }
            if (!Directory.Exists(pathString4))
            {
                Directory.CreateDirectory(pathString4);
            }
            if (!Directory.Exists(pathString5))
            {
                Directory.CreateDirectory(pathString5);
            }

            // Проверяем был ли загружен фаил
            if (file != null && file.ContentLength > 0)
            {
                string ext = file.ContentType.ToLower();

                // Проверяем расширение файла
                if (ext != "image/jpg"      &&
                    ext != "image/jpeg"     &&
                    ext != "image/pjpeg"    &&
                    ext != "image/png"      &&
                    ext != "image/gif"       )
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image was not uploaded = wrong image extension");
                        return View(model);
                    }
                }



                // Объявляем переменную с именем изображения
                string imgName = file.FileName;
                // Сохраняем имя изображения в модель
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);

                    dto.ImageName = imgName;
                    db.SaveChanges();
                }
                // Назначаем пути к оригинальному и уменьшеному изображению
                var pathStr2 = string.Format($"{pathString2}\\{imgName}");
                var pathStr3 = string.Format($"{pathString3}\\{imgName}");
                // Сохранить оригинальное изображение
                file.SaveAs(pathStr2);
                // Создаем и сохраняем уменьшеную копию
                WebImage img = new WebImage(file.InputStream);
                img.Resize(300, 300).Crop(1, 1);
                img.Save(pathStr3);
            }
            #endregion



            return RedirectToAction("AddProduct");
        }
        // GET: Admin/Shop/Products
        [HttpGet]
        public ActionResult Products(int? page, int? catId) //int? означает что может принимать null
        {
            List<ProductVM> ListOfProductVM;

            var pageNumber = page ?? 1;

            using (Db db = new Db())
            {
                ListOfProductVM = db.Products.ToArray()
                    .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                    .Select(x => new ProductVM(x))
                    .ToList();

                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                ViewBag.SelectedCat = catId.ToString();
            }

            var onePageOfProducts = ListOfProductVM.ToPagedList(pageNumber, 3);

            ViewBag.onePageOfProducts = onePageOfProducts;

            return View(ListOfProductVM);
        }
        // GET: Admin/Shop/EditProduct/id
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            ProductVM model;

            using(Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                if(dto == null)
                {
                    return Content("That product does not exist.");
                }

                model = new ProductVM(dto);

                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));
            }
            return View(model);
        }
        // POST: Admin/Shop/EditProduct/id
        [HttpPost]
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file)
        {
            int id = model.Id;

            using(Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));

            if(!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                if(db.Products.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That product name is taken.");
                    return View(model);
                }
            }

            using(Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);

                dto.Name = model.Name;
                dto.longDesc = model.Name.Replace(" ", "-").ToLower();
                dto.shortDesc = model.shortDesc;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;

                CategoryDTO catDto = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDto.Name;

                db.SaveChanges();
            }

            TempData["SM"] = "You have edited the product.";

            #region Image Upload

            if(file != null && file.ContentLength > 0)
            {
                string ext = file.ContentType.ToLower();

                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/png" &&
                    ext != "image/gif")
                {
                    using (Db db = new Db())
                    {                        
                        ModelState.AddModelError("", "The image was not uploaded = wrong image extension");
                        return View(model);
                    }
                }

                var OriginalDir = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
               
                var pathString1 = Path.Combine(OriginalDir.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(OriginalDir.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (var file2 in di1.GetFiles())
                {
                    file2.Delete();
                }
                foreach (var file3 in di2.GetFiles())
                {
                    file3.Delete();
                }

                string imgName = file.FileName;

                using(Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);

                     dto.ImageName = imgName;
                    
                    db.SaveChanges();
                }

                var pathStr2 = string.Format($"{pathString1}\\{imgName}");
                var pathStr3 = string.Format($"{pathString2}\\{imgName}");
                // Сохранить оригинальное изображение
                file.SaveAs(pathStr2);
                // Создаем и сохраняем уменьшеную копию
                WebImage img = new WebImage(file.InputStream);
                img.Resize(300, 300).Crop(1,1);
                img.Save(pathStr3);
            }

            #endregion

            return RedirectToAction("EditProduct");
        }
        // GET: Admin/Shop/DeleteProduct/id
        [HttpGet]
        public ActionResult DeleteProduct(int id)
        {
            using(Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);

                db.Products.Remove(dto);
                db.SaveChanges();
            }

            var OriginalDir = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));           
            var pathString = Path.Combine(OriginalDir.ToString(), "Products\\" + id.ToString());

            if(Directory.Exists(pathString)) //Удаляем каталог (true - все подкаталоги)
            {
                Directory.Delete(pathString, true);
            }

            return RedirectToAction("Products");
        }
        // POST: Admin/Shop/SaveGalleryImages/id
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            foreach(string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];

                if(file != null && file.ContentLength > 0)
                {
                    var OriginalDir = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
                    var pathString1 = Path.Combine(OriginalDir.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    var pathString2 = Path.Combine(OriginalDir.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    var path1 = string.Format($"{pathString1}\\{file.FileName}");
                    var path2 = string.Format($"{pathString2}\\{file.FileName}");

                    file.SaveAs(path1);

                    WebImage img = new WebImage(file.InputStream);

                    img.Resize(300, 300).Crop(1,1);
                    img.Save(path2);
                }
            }
        }
        // POST: Admin/Shop/DeleteImage/id/imageName        
        public void DeleteImage(int id, string imageName)
        {
            string fullPath1 = Request.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/" + imageName);
            string fullPath2 = Request.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs/" + imageName);


            if (System.IO.File.Exists(fullPath1))
            {
                System.IO.File.Delete(fullPath1);
            }
            if (System.IO.File.Exists(fullPath2))
            {
                System.IO.File.Delete(fullPath2);
            }
        }

        

    }
}