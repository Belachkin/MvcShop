using MvcShop.Models.Data;
using MvcShop.Models.ViewModels.Account;
using MvcShop.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcShop.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart       
        public ActionResult Index()
        {
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            if(cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "You cart is empty.";
                return View();
            }

            decimal total = 0m;

            foreach(var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            return View("Index", cart);
        }

        public ActionResult CartPartial()
        {
            CartVM model = new CartVM();

            int qty = 0;

            decimal price = 0m;

            if(Session["cart"] != null)
            {
                var list = (List<CartVM>)Session["cart"];

                foreach(var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                model.Quantity = qty;
                model.Price = price;

            }
            else
            {
                model.Quantity = 0;
                model.Price = 0m;
            }

            return PartialView("_CartPartial", model);
        }

        public ActionResult AddToCartPartial(int id)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            CartVM model = new CartVM();

            using(Db db = new Db())
            {
                ProductDTO productDTO = db.Products.Find(id);

                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                if(productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = productDTO.Id,
                        ProductName = productDTO.Name,
                        Quantity = 1, 
                        Price = productDTO.Price,
                        Image = productDTO.ImageName
                    });
                }
                else
                {
                    productInCart.Quantity++;
                }

            }

            int qty = 0;
            decimal price = 0m;

            foreach(var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            Session["cart"] = cart;

            return PartialView("_AddToCartPartial", model);
        }      
        public JsonResult IncrementProduct(int productId)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using(Db db = new Db())
            {
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                model.Quantity++;

                var result = new { qty = model.Quantity, price = model.Price };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DecrementProduct(int productId)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                if(model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                var result = new { qty = model.Quantity, price = model.Price };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public void RemoveProduct(int productId)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using(Db db = new Db())
            {
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                cart.Remove(model);
            }
        }
        [HttpGet]
        public ActionResult Checkout()
        {
            string userName = User.Identity.Name;
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            if (cart.Count == 0 || Session["cart"] == null)
            {               
                return RedirectToAction("Index");
            }

            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }


            ViewBag.AllItems = cart.Count;
            ViewBag.GrandTotal = total;
           

            using(Db db = new Db())
            {
                UserDTO userDTO = db.Users.FirstOrDefault(x => x.Username == userName);

                ViewBag.Username = userDTO.Username;
                ViewBag.LastName = userDTO.LastName;
                ViewBag.FirstName = userDTO.FirstName;
                ViewBag.Email = userDTO.EmailAdress;
            }



            return View("Checkout", cart);
        }
        [HttpPost]
        public ActionResult Checkout(string fname, string lname, string email, 
                                    string address, string country, string state, string zip)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            string userName = User.Identity.Name;            

            using(Db db = new Db())
            {
                OrderDTO orderDTO = new OrderDTO();

                var q = db.Users.FirstOrDefault(x => x.Username == userName);
                int userId = q.Id;

                orderDTO.UserId = userId;
                
                orderDTO.CreatedAt = DateTime.Now;

                foreach(var item in cart)
                {
                    orderDTO.ProductInfo += $"{item.ProductName} x{item.Quantity}";
                }

                orderDTO.DeliveryInfo = $"{fname} {lname} \n{email}\n{country} {state} {zip} \n{address} ";
                orderDTO.DeliveryStatus = "Waiting to ship";

                db.Orders.Add(orderDTO);
                db.SaveChanges();
            }

            return RedirectToAction("Profile", "Account");

        }
    }
}