using MvcShop.Models.Data;
using MvcShop.Models.ViewModels.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcShop.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        // GET: Admin/Orders
        public ActionResult Index()
        {
            List<OrderVM> orderDTO;
            
            using(Db db = new Db())
            {
                orderDTO = db.Orders.ToArray().Select(x => new OrderVM(x)).ToList();
                
                foreach(var order in orderDTO)
                {
                    UserDTO dto = db.Users.FirstOrDefault(x => x.Id == order.UserId);
                    order.Username = dto.Username;
                }
            }

            return View(orderDTO);
        }
    }
}