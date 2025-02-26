using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OfficeFoodAPI.Model
{
    public class Vendor
    {
        [Key]
        public Guid vendorid { get; set; }
        public string name { get; set; }
        public List<string> serviceareas { get; set; } // should we store this data somewhere else

        [JsonIgnore]  // Prevent circular dependency
        public List<MenuItem> menu_item { get; set; }
        public DateTime createdat { get; set; }
        public DateTime upatedat { get; set; }
    }

    public class Vendor_post
    {
        public string? name { get; set; }
        public List<string>? serviceareas { get; set; } // should we store this data somewhere else
        //public List<MenuItem>? menu_item { get; set; }
    }

    public class MonthlyReport
    {
        public double TotalMonthlyRevenue { get; set; }
        public Dictionary<string, double> ItemWiseTotal { get; set; }
        public Dictionary<int, double> DayWiseTotal { get; set; }
        public int TotalPlates { get; set; }
    }

    public class CompanyMonthlyReport
    {
        public string CompanyName { get; set; }
        public int TotalPlates { get; set; }
        public double TotalBill { get; set; }
    }

    public class CompanyMonthlyReportSummary
    {
        public List<CompanyMonthlyReport> Companies { get; set; }
        public double TotalOverallMonthlyRevenue { get; set; }
    }

    public class DateWiseReport
    {
        public Guid CompanyId { get; set; }
        public Guid VendorId { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalPlates { get; set; }
        public double TotalRevenue { get; set; }
        public Dictionary<string, int> ItemWiseCounts { get; set; } = new();
    }


}
