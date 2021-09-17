using MvcShop.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcShop.Models.ViewModels.Shop
{
    public class CategoryVM
    {

        public CategoryVM() { }
        public CategoryVM(CategoryDTO row) 
        {
            Id = row.Id;
            Name = row.Name;
            Desc = row.Desc;
            Sorting = row.Sorting;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public int Sorting { get; set; }
    }
}