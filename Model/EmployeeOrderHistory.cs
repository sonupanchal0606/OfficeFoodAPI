using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OfficeFoodAPI.Model
{
    public class EmployeeOrderHistory
    {
        [Key]
        public Guid employeeid { get; set; }

        [ForeignKey("User")]
        public Guid userid { get; set; }
        public User user { get; set; }

        [ForeignKey("Company")]
        public Guid companyid { get; set; }

        [JsonIgnore]
        public Company company { get; set; } // navgational property. one employee associated with one company

        public int year { get; set; } //= DateTime.Now.AddMonths(1);
        public int month { get; set; }
        public OrderCode day1 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>?  day1logs { get; set; } // Store logs as JSONB
        public OrderCode day2 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day2logs { get; set; } // Store logs as JSONB
        public OrderCode day3 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day3logs { get; set; } // Store logs as JSONB
        public OrderCode day4 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day4logs { get; set; } // Store logs as JSONB
        public OrderCode day5 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day5logs { get; set; } // Store logs as JSONB
        public OrderCode day6 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day6logs { get; set; } // Store logs as JSONB
        public OrderCode day7 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day7logs { get; set; } // Store logs as JSONB
        public OrderCode day8 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day8logs { get; set; } // Store logs as JSONB
        public OrderCode day9 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day9logs { get; set; } // Store logs as JSONB
        public OrderCode day10 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day10logs { get; set; } // Store logs as JSONB
        public OrderCode day11 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day11logs { get; set; } // Store logs as JSONB
        public OrderCode day12 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day12logs { get; set; } // Store logs as JSONB
        public OrderCode day13 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day13logs { get; set; } // Store logs as JSONB
        public OrderCode day14 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day14logs { get; set; } // Store logs as JSONB
        public OrderCode day15 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day15logs { get; set; } // Store logs as JSONB
        public OrderCode day16 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day16logs { get; set; } // Store logs as JSONB
        public OrderCode day17 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day17logs { get; set; } // Store logs as JSONB
        public OrderCode day18 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day18logs { get; set; } // Store logs as JSONB
        public OrderCode day19 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day19logs { get; set; } // Store logs as JSONB
        public OrderCode day20 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day20logs { get; set; } // Store logs as JSONB
        public OrderCode day21 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day21logs { get; set; } // Store logs as JSONB
        public OrderCode day22 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day22logs { get; set; } // Store logs as JSONB
        public OrderCode day23 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day23logs { get; set; } // Store logs as JSONB
        public OrderCode day24 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day24logs { get; set; } // Store logs as JSONB
        public OrderCode day25 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day25logs { get; set; } // Store logs as JSONB
        public OrderCode day26 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day26logs { get; set; } // Store logs as JSONB
        public OrderCode day27 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day27logs { get; set; } // Store logs as JSONB
        public OrderCode day28 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day28logs { get; set; } // Store logs as JSONB
        public OrderCode day29 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day29logs { get; set; } // Store logs as JSONB
        public OrderCode day30 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day30logs { get; set; } // Store logs as JSONB
        public OrderCode day31 { get; set; } = OrderCode.NoOrder;
        [Column(TypeName = "jsonb")]
        public List<Log>? day31logs { get; set; } // Store logs as JSONB
        public DateTime createdat { get; set; }
        public DateTime upatedat { get; set; }

    }

    /*
     Food Order Type : 
     ** Subscribed (S), Unsubscribed (U)
     ** Veg (V), Non Veg (N), Veg Special (Vs), Non Veg Special (Ns)
     ** Charged on Company (C), Charged on Employee (E)
     */
    public enum OrderCode 
    {
        NoOrder = 0,
        OrderCancelled = 1,
        SVC = 2,
        SVE = 3,
        SNC = 4,
        SNE = 5,
        SVsC = 6,
        SVsE = 7,
        SNsC = 8,
        SNsE = 9,
        UVC = 10,
        UVE = 11,
        UNC = 12,
        UNE = 13,
        UVsC = 14,
        UVsE = 15,
        UNsC = 16,
        UNsE = 17,
    }


    public class Log
    {
        public DateTime requestedat { get; set; }
        public DateTime? cancelledat { get; set; }
        public string message { get; set; }
    }


    public class EmployeeOrderHistory_Report
    {
        public Guid employeeid { get; set; }
        public Guid userid { get; set; }
        public User user { get; set; }
        public Guid companyid { get; set; }
        public Company company { get; set; } // navgational property. one employee associated with one company
        public int year { get; set; }
        public int month { get; set; }
        public List<DailyOrderReport> OrderDetails { get; set; }

        // derived calculations
        public double TotalPrice { get; set; }
        public double EmployeePaid { get; set; }
        public double CompanyPaid { get; set; }
        public int TotalOrders { get; set; }
        public int CancelledOrNoOrders { get; set; }
        
    }

    public class DailyOrderReport
    {
        public int Day { get; set; }
        public OrderCode OrderStatus { get; set; }
        public List<Log> Logs { get; set; }
    }

    public class PlateSubscriptionRequest
    {
        public Guid userid { get; set; }
        public Guid CompanyId { get; set; }
        public Guid VendorId { get; set; }
        public Guid ItemId { get; set; }

        public int month { get; set; }

        public int year { get; set; }   
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }  // If subscribing for a month, use last day of the month
        public bool IsChargedToCompany { get; set; }  // True -> Company Pays, False -> Employee Pays
    }

    public class PlaceOrderRequest 
    {
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid ItemId { get; set; }
    }

    public class CancelOrderRequest
    {
        public Guid UserId { get; set; }
    }

    /* public class EmployeeMap 
     {
         public static EmployeeOrderHistory_Report MapEmptoDTO(EmployeeOrderHistory e)
         {
             return new EmployeeOrderHistory_Report
             {
                 employeeid = e.employeeid,
                 userid = e.userid,
                 user = e.user,
                 companyid = e.companyid,
                 company = e.company,
                 year = e.year,
                 month = e.month,
                 day1 = e.day1,
                 day1logs = e.day1logs,
                 day2 = e.day2,
                 day2logs = e.day2logs,
                 day3 = e.day3,
                 day3logs = e.day3logs,
                 day4 = e.day4,
                 day4logs = e.day4logs,
                 day5 = e.day5,
                 day5logs = e.day5logs,
                 day6 = e.day6,
                 day6logs = e.day6logs,
                 day7 = e.day7,
                 day7logs = e.day7logs,
                 day8 = e.day8,
                 day8logs = e.day8logs,
                 day9 = e.day9,
                 day9logs = e.day9logs,
                 day10 = e.day10,
                 day10logs = e.day10logs,
                 day11 = e.day11,
                 day11logs = e.day11logs,
                 day12 = e.day12,
                 day12logs = e.day12logs,
                 day13 = e.day13,
                 day13logs = e.day13logs,
                 day14 = e.day14,
                 day14logs = e.day14logs,
                 day15 = e.day15,
                 day15logs = e.day15logs,
                 day16 = e.day16,
                 day16logs = e.day16logs,
                 day17 = e.day17,
                 day17logs = e.day17logs,
                 day18 = e.day18,
                 day18logs = e.day18logs,
                 day19 = e.day19,
                 day19logs = e.day19logs,
                 day20 = e.day20,
                 day20logs = e.day20logs,
                 day21 = e.day21,
                 day21logs = e.day21logs,
                 day22 = e.day22,
                 day22logs = e.day22logs,
                 day23 = e.day23,
                 day23logs = e.day23logs,
                 day24 = e.day24,
                 day24logs = e.day24logs,
                 day25 = e.day25,
                 day25logs = e.day25logs,
                 day26 = e.day26,
                 day26logs = e.day26logs,
                 day27logs = e.day27logs,
                 day28 = e.day28,
                 day28logs = e.day28logs,
                 day29 = e.day29,
                 day29logs = e.day29logs,
                 day30 = e.day30,
                 day30logs = e.day30logs,
                 day31 = e.day31,
                 day31logs = e.day31logs,
                 createdat = e.createdat,
                 updatedat = e.updatedat,
 *//*              totalPlateCountOfEmpgroupedByThali = e.totalPlateCountOfEmpgroupedByThali,
                 totalPriceGroupedByThali = e.totalPriceGroupedByThali,
                 numberOfDaysFoodTaken = e.numberOfDaysFoodTaken,
                 numberOdDaysFoodNotTaken = e.numberOdDaysFoodNotTaken,
                 cancelledOrder = e.cancelledOrder*//*
             };
         }
     }*/
    public class MonthlyReportDto
    {
        public Guid UserId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public List<DailyOrderDetails> DailyOrders { get; set; } = new List<DailyOrderDetails>();
        public int TotalPlates { get; set; }
        public double TotalBill { get; set; }
        public double EmployeePaidPercentage { get; set; }
        public double CompanyPaidPercentage { get; set; }
    }

    public class DailyOrderDetails
    {
        public DateTime Date { get; set; }
        public string FoodItem { get; set; }
        public double Price { get; set; }
        public double EmployeePaid { get; set; }
        public double CompanyPaid { get; set; }
    }

}
