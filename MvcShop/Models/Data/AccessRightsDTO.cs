using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MvcShop.Models.Data
{
    [Table("AccessRights")]
    public class AccessRightsDTO
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}