using MvcShop.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcShop.Models.ViewModels.Shop
{
    public class ProductVM
    {

        public ProductVM() { }
        public ProductVM(ProductDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            shortDesc = row.shortDesc;
            longDesc = row.longDesc;
            Price = row.Price;
            CategoryName = row.CategoryName;
            CategoryId = row.CategoryId;
            ImageName = row.ImageName;
        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string shortDesc { get; set; }
        [Required]
        [DisplayName("Description")]
        public string longDesc { get; set; }
        [Required]
        public decimal Price { get; set; }     
        public string CategoryName { get; set; }
        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        [DisplayName("Image")]
        public string ImageName { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<string> GalleryImages { get; set; }
    }
}