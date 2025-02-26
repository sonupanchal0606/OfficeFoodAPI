using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeFoodAPI.Model
{
    public class MenuItem
    {
        [Key]
        public Guid menuitemid { get; set; }
        public string itemname { get; set; } // veg thali, non veg thali, special veg thali, special non veg thali
        public double price { get; set; }

        [ForeignKey("vendorid")]
        public Guid vendorid { get; set; }
        public Vendor vendor { get; set; }
        public DateTime createdat { get; set; }
        public DateTime updatedat { get; set; }
    }

    public class MenuItem_post
    {
        [Key]
        public string? itemname { get; set; } // veg thali, non veg thali, special veg thali, special non veg thali
        public double? price { get; set; }
        public Guid? vendorid { get; set; }
    }

    public class MenuItem_return_dto
    {

        public Guid? menuitemid { get; set; }
        public string itemname { get; set; } // veg thali, non veg thali, special thali veg, special thali non veg
        public double? price { get; set; }
        public DateTime? createdat { get; set; }
        public DateTime? upatedat { get; set; }
        public Guid? vendorid { get; set; }
    }

}
