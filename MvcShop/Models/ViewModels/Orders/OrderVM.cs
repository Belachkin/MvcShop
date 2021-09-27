using MvcShop.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcShop.Models.ViewModels.Orders
{
    public class OrderVM
    {
        public OrderVM() { }
        public OrderVM(OrderDTO row) 
        {
            OrderId = row.OrderId;
            UserId = row.UserId;
            ProductInfo = row.ProductInfo;
            DeliveryInfo = row.DeliveryInfo;
            CreatedAt = row.CreatedAt;
            DeliveryStatus = row.DeliveryStatus;
         
        }

        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string ProductInfo { get; set; }
        public string DeliveryInfo { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DeliveryStatus { get; set; }
        public string Username { get; set; }
    }
}