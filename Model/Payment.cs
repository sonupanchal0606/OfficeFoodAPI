using System.ComponentModel.DataAnnotations;

namespace OfficeFoodAPI.Model
{
    public class Payment
    {
        [Key]
        public Guid paymentid { get; set; }
        public Guid companyid { get; set; }
        public Guid vendorid { get; set; }
        public decimal amount { get; set; }
        public string status { get; set; } // Pending, Completed
        public DateTime date { get; set; }
        public DateTime createdat { get; set; }
        public DateTime upatedat { get; set; }
    }
}
