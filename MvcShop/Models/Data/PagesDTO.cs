using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MvcShop.Models.Data
{
    [Table("Pages")]
    public class PagesDTO
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string shortDesc { get; set; }
        public string Body { get; set; }
        public int Sorting { get; set; }
        public bool HasSidebar { get; set; }

    }
}