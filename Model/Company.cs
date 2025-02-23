using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeFoodAPI.Model
{
    public class Company
    {
        [Key]
        public Guid companyid { get; set; }
        public string companyname { get; set; }
        public string location { get; set; } // same location as served by vendor
        public double subsidyperplate { get; set; } // in %
        //public List<Guid> employeeuserids { get; set; } // userids of employees in a company

        [ForeignKey("vendorid")]
        public Guid? vendorid { get; set; }  
        public Vendor? vendor { get; set; }  

        public DateTime createdat { get; set; }
        public DateTime upatedat { get; set; }
    }

    public class Company_post
    {
        public string? name { get; set; }
        public string? location { get; set; } 
        public double? subsidyperplate { get; set; } 
        public Guid? vendorid { get; set; }
    }

    public class Company_return_dto
    {
        public Guid companyid { get; set; }
        public string? name { get; set; }
        public string? location { get; set; }
        public double? subsidyperplate { get; set; }
        public Guid vendorid { get; set; }
        public DateTime createdat { get; set; }
        public DateTime upatedat { get; set; }
    }

    public class CompanyConversion
    {
        public static Company_return_dto MapCompanyToDto(Company c)
        {
            return new Company_return_dto
            {
                companyid = c.companyid,
                name = c.companyname,
                location = c.location,
                subsidyperplate = c.subsidyperplate,
                vendorid = c.vendorid.Value,
                createdat = c.createdat,
                upatedat = c.upatedat,
            };
        }
    }

    public class MonthlyEmployeeReport
    {
        public Guid CompanyId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public List<EmployeeReportItem> EmployeeRecords { get; set; } = new();
        public int TotalPlates { get; set; }
        public double TotalBill { get; set; }
        public double TotalCompanyPaid { get; set; }
        public double TotalEmployeePaid { get; set; }
    }

    public class EmployeeReportItem
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int TotalPlates { get; set; }
        public double TotalBill { get; set; }
        public double CompanyPaid { get; set; }
        public double EmployeePaid { get; set; }
    }

    public class DailyCompanyReport
    {
        public string Date { get; set; }
        public Guid CompanyId { get; set; }
        public Dictionary<string, int> ItemWisePlateCount { get; set; }
        public double TotalBill { get; set; }
        public double TotalCompanyPaid { get; set; }
        public double TotalEmployeePaid { get; set; }
    }

}
