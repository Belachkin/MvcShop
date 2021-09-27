using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MvcShop.Models.Data
{
    [Table("Orders")]
    public class OrderDTO
    {
        [Key]
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string ProductInfo { get; set; }        
        public string DeliveryInfo { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DeliveryStatus { get; set; }


    }
}