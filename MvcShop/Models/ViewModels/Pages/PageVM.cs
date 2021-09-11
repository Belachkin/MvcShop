using MvcShop.Models.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcShop.Models.ViewModels.Pages
{
    public class PageVM
    {
        public PageVM() { }
        public PageVM(PagesDTO row)
        {
            Id = row.Id;
            Title = row.Title;
            shortDesc = row.shortDesc;
            Body = row.Body;
            Sorting = row.Sorting;
            HasSidebar = row.HasSidebar;
        }

        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }
        public string shortDesc { get; set; }
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3)]
        public string Body { get; set; }
        public int Sorting { get; set; }
        public bool HasSidebar { get; set; }

    }
}