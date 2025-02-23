using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OfficeFoodAPI.Data;
using OfficeFoodAPI.Model;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace OfficeFoodAPI.Handlers
{
    public class CompanyHandler
    {
        private FoodDbContext _context;

        public CompanyHandler(FoodDbContext context)
        {
            _context = context;
        }

        public async Task<List<Company>> GetAll()
        {
            var list = await _context.company_mstr.ToListAsync();
            if (list == null)
            {
                throw new Exception("No records found");
            }
            return list;
        }

        public async Task<Company_return_dto> Get(Guid id)
        {
            var record = await _context.company_mstr.Where(c => c.companyid == id).FirstOrDefaultAsync();
            if (record == null)
            {
                throw new Exception("No records found");
            }

            return CompanyConversion.MapCompanyToDto(record);
        }


        public async Task<Company> Post(Company_post value)
        {
            Company company = new Company()
            {
                companyname = value.name,
                location = value.location,
                subsidyperplate = value.subsidyperplate.Value,
                vendorid = value.vendorid != null? value.vendorid : null,
                createdat = DateTime.UtcNow,
                upatedat = DateTime.UtcNow,
            };
            await _context.company_mstr.AddAsync(company);
            await _context.SaveChangesAsync();

            var record = await _context.company_mstr.FindAsync(company.companyid);
            if (record == null)
            {
                throw new Exception("Error in saving records");
            }

            return record;
        }



        public async Task<Company> Put(Guid id, Company_post value)
        {
            var data = await _context.company_mstr.Where(c => c.companyid == id).FirstOrDefaultAsync();
            if (data == null)
            {
                throw new Exception("No record to update");
            }
            if (value.name != null)
            {
                data.companyname = value.name;
            }
            if (value.location != null)
            {
                data.location = value.location;
            }
            if (value.subsidyperplate != null)
            {
                data.subsidyperplate = value.subsidyperplate.Value;
            }
            if (value.vendorid != null)
            {
                data.vendorid = value.vendorid.Value;
            }
            data.upatedat = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return data;
        }


        public async Task<Object> GetCompanyVendorDetails(Guid companyid)
        {
            var data = await _context.company_mstr.Where(c => c.companyid == companyid).Include(v => v.vendor).FirstOrDefaultAsync();
            if (data == null)
            {
                throw new Exception("No company found with this id");
            }
            if (data.vendor == null)
            {
                throw new Exception("No vendor found for this company");
            }
            return new
            {
                companyname = data.companyname,
                vendor_details = data.vendor,
            };
        }

        public async Task<Object> AssignVendor(Guid companyid, Guid vendorid)
        {
            var data = await _context.company_mstr.Where(c => c.companyid == companyid).Include(v => v.vendor).FirstOrDefaultAsync();
            data.vendorid = vendorid;
            await _context.SaveChangesAsync();

            return data;
        }

        public async Task<Object> AddEmployeeUser(Guid companyid, string emailid)
        {
            // add employee in user_mstr and employee_mstr
            var usertypeemp = await _context.usertype_mstr.Where(u => u.usertype == "employee").FirstOrDefaultAsync();
            var company = await _context.company_mstr.Where(c => c.companyid == companyid).FirstOrDefaultAsync();
            User newuser = new User()
            {
                name = emailid.Split('.')[0],
                email = emailid,
                companyid = companyid,
                Company = company,
                usertypeid = usertypeemp.usertypeid,
                usertype = usertypeemp,
                address = company.location,
                createdat = DateTime.Now,
                upatedat = DateTime.Now,
            };
            await _context.AddAsync(newuser);
            await _context.SaveChangesAsync();

            EmployeeOrderHistory emp = new EmployeeOrderHistory()
            {
                employeeid = newuser.userid,
                companyid = company.companyid,
                year = DateTime.Now.Year,

            };

            return newuser;
        }


        public async Task<JsonResult> RemoveEmployeeUser(Guid userid)
        {
            // add employee in user_mstr and employee_mstr
            var user = await _context.user_mstr.FindAsync(userid);
            _context.Remove(user);

            var emp = await _context.employee_history_mstr.Where(e => e.userid == userid).ToListAsync();

            _context.RemoveRange(emp);
            await _context.SaveChangesAsync();

            return new JsonResult("employee removed");
        }


        public async Task<(User u, List<EmployeeOrderHistory> e)> GetEmployeeDetails(string emailid)
        {
            var user = await _context.user_mstr.Where(e => e.email == emailid).FirstOrDefaultAsync();
            var empHistory = await _context.employee_history_mstr.Where(e => e.userid == user.userid).ToListAsync();
            return (user, empHistory);
        }



        public async Task<EmployeeOrderHistory_Report> GetMonthlyReportOfAnEmployee(Guid companyid, Guid userid, int month, int year)
        {
            var employeeHistory = await _context.employee_history_mstr
                .Where(e => e.month == month && e.year == year && e.companyid == companyid && e.userid == userid)
                .Include(c => c.company)
                    .ThenInclude(v => v.vendor)
                    .ThenInclude(m => m.menu_item)
                .Include(c => c.user)
                .FirstOrDefaultAsync();

            if (employeeHistory == null)
                return null; // No records found

            var report = new EmployeeOrderHistory_Report
            {
                userid = userid,
                user = employeeHistory.user,
                companyid = companyid,
                company = employeeHistory.company,
                year = year,
                month = month,
                TotalOrders = 0,
                EmployeePaid = 0,
                CompanyPaid = 0,
                CancelledOrNoOrders = 0,
                TotalPrice = 0,
                OrderDetails = new List<DailyOrderReport>()
            };

            // Get subsidy percentage from company table
            double subsidyPercentage = employeeHistory.company?.subsidyperplate ?? 0;

            // Fetch menu item prices dynamically
            var menuPrices = employeeHistory.company?.vendor?.menu_item.ToDictionary(m => m.itemname, m => m.price) ?? new Dictionary<string, double>();

            for (int day = 1; day <= 31; day++)
            {
                var orderStatusProp = typeof(EmployeeOrderHistory).GetProperty($"day{day}");
                if (orderStatusProp == null) break;

                var logsProp = typeof(EmployeeOrderHistory).GetProperty($"day{day}logs");

                var orderStatus = (OrderCode)orderStatusProp.GetValue(employeeHistory);
                var logs = (List<Log>)logsProp.GetValue(employeeHistory);

                if (orderStatus == OrderCode.NoOrder || orderStatus == OrderCode.OrderCancelled)
                {
                    report.CancelledOrNoOrders++;
                }
                else
                {
                    report.TotalOrders++;

                    // Determine menu item companyname based on order status
                    string itemName = GetItemNameFromOrderCode(orderStatus);
                    if (!string.IsNullOrEmpty(itemName) && menuPrices.TryGetValue(itemName, out double price))
                    {
                        report.TotalPrice += price;

                        if (orderStatus.ToString().EndsWith("E")) // Charged on Employee (company + employee)
                        {
                            double companyShare = (subsidyPercentage / 100) * price;
                            double employeeShare = price - companyShare;

                            report.EmployeePaid += (int)employeeShare;
                            report.CompanyPaid += (int)companyShare;
                        }
                        else if (orderStatus.ToString().EndsWith("C")) // Charged on Company (fully subsidized)
                        {
                            report.EmployeePaid += 0;
                            report.CompanyPaid += price;
                        }
                    }
                }

                report.OrderDetails.Add(new DailyOrderReport
                {
                    Day = day,
                    OrderStatus = orderStatus,
                    Logs = logs ?? new List<Log>()
                });
            }

            return report;
        }

        public async Task<MonthlyEmployeeReport> GetMonthlyEmployeeReport(Guid companyId, int month, int year)
        {
            // Step 1: Fetch all employee orders for the given company and month
            var employees = await _context.employee_history_mstr
                .Where(e => e.companyid == companyId && e.month == month && e.year == year)
                .Include(e => e.user)
                .Include(e => e.company)
                .ThenInclude(v => v.vendor)
                .ThenInclude(m => m.menu_item)
                .ToListAsync();

            if (!employees.Any()) return null;

            // Step 2: Get subsidy percentage from company table
            double subsidyPercentage = employees.FirstOrDefault()?.company?.subsidyperplate ?? 0;

            // Step 3: Fetch menu item prices
            var menuPrices = employees.FirstOrDefault()?.company?.vendor?.menu_item.ToDictionary(m => m.itemname, m => m.price) ?? new();

            // Step 4: Initialize report
            List<EmployeeReportItem> employeeRecords = new();
            int totalPlates = 0;
            double totalBill = 0, totalCompanyPaid = 0, totalEmployeePaid = 0;

            foreach (var employee in employees)
            {
                int employeeTotalPlates = 0;
                double employeeTotalBill = 0, employeePaid = 0, companyPaid = 0;

                for (int day = 1; day <= 31; day++)
                {
                    var orderStatusProp = typeof(EmployeeOrderHistory).GetProperty($"day{day}");
                    if (orderStatusProp == null) continue;

                    var orderStatus = (OrderCode)orderStatusProp.GetValue(employee);
                    if (orderStatus == OrderCode.NoOrder || orderStatus == OrderCode.OrderCancelled)
                        continue;

                    // Determine menu item type from OrderCode
                    string itemName = GetItemNameFromOrderCode(orderStatus);
                    if (string.IsNullOrEmpty(itemName) || !menuPrices.ContainsKey(itemName))
                        continue;

                    double price = menuPrices[itemName];
                    employeeTotalBill += price;
                    employeeTotalPlates++;

                    if (orderStatus.ToString().EndsWith("E")) // Employee pays a share
                    {
                        double companyShare = (subsidyPercentage / 100) * price;
                        double employeeShare = price - companyShare;

                        employeePaid += employeeShare;
                        companyPaid += companyShare;
                    }
                    else if (orderStatus.ToString().EndsWith("C")) // Company pays full
                    {
                        employeePaid += 0;
                        companyPaid += price;
                    }
                }

                // Add to list
                employeeRecords.Add(new EmployeeReportItem
                {
                    EmployeeId = employee.userid,
                    EmployeeName = employee.user?.name ?? "Unknown",
                    TotalPlates = employeeTotalPlates,
                    TotalBill = employeeTotalBill,
                    CompanyPaid = companyPaid,
                    EmployeePaid = employeePaid
                });

                // Aggregate totals
                totalPlates += employeeTotalPlates;
                totalBill += employeeTotalBill;
                totalCompanyPaid += companyPaid;
                totalEmployeePaid += employeePaid;
            }

            return new MonthlyEmployeeReport
            {
                CompanyId = companyId,
                Month = month,
                Year = year,
                EmployeeRecords = employeeRecords,
                TotalPlates = totalPlates,
                TotalBill = totalBill,
                TotalCompanyPaid = totalCompanyPaid,
                TotalEmployeePaid = totalEmployeePaid
            };
        }

        private string GetItemNameFromOrderCode(OrderCode orderCode)
        {
            return orderCode switch
            {
                OrderCode.SVC or OrderCode.UVC => "Veg Thali",
                OrderCode.SVE or OrderCode.UVE => "Veg Thali",
                OrderCode.SNC or OrderCode.UNC => "Non-Veg Thali",
                OrderCode.SNE or OrderCode.UNE => "Non-Veg Thali",
                OrderCode.SVsC or OrderCode.UVsC => "Special Veg Thali",
                OrderCode.SVsE or OrderCode.UVsE => "Special Veg Thali",
                OrderCode.SNsC or OrderCode.UNsC => "Special Non-Veg Thali",
                OrderCode.SNsE or OrderCode.UNsE => "Special Non-Veg Thali",
                _ => string.Empty // Return empty if the OrderCode is unrecognized
            };
        }


        public async Task<DailyCompanyReport> GetDailyCompanyReport(Guid companyId, int day, int month, int year)
        {
            // Fetch employee orders for the given company and date
            var employeeOrders = await _context.employee_history_mstr
                .Where(e => e.companyid == companyId && e.month == month && e.year == year)
                .ToListAsync();

            if (employeeOrders == null || !employeeOrders.Any())
                return null;

            // Fetch menu item prices
            var menuPrices = await _context.menuitem_mstr
                .ToDictionaryAsync(m => m.itemname, m => m.price);

            // Fetch company subsidy percentage
            var company = await _context.company_mstr.FindAsync(companyId);
            double subsidyPercentage = company?.subsidyperplate ?? 0;

            // Initialize report data
            Dictionary<string, int> itemWisePlateCount = new();
            double totalBill = 0, totalCompanyPaid = 0, totalEmployeePaid = 0;

            // Process orders for the given day
            foreach (var order in employeeOrders)
            {
                var orderProperty = typeof(EmployeeOrderHistory).GetProperty($"day{day}");
                if (orderProperty == null) continue;

                var orderCode = (OrderCode)orderProperty.GetValue(order);
                if (orderCode == OrderCode.NoOrder || orderCode == OrderCode.OrderCancelled) continue;

                // Extract menu item type from OrderCode (assuming it directly maps to item name)
                string menuItemName = orderCode.ToString().Substring(0, orderCode.ToString().Length - 1);

                // Update plate count
                if (!itemWisePlateCount.ContainsKey(menuItemName))
                    itemWisePlateCount[menuItemName] = 0;
                itemWisePlateCount[menuItemName]++;

                // Get item price
                double price = menuPrices.ContainsKey(menuItemName) ? menuPrices[menuItemName] : 0;
                totalBill += price;

                // Calculate company and employee contribution
                if (orderCode.ToString().EndsWith("E")) // Employee pays (company + employee)
                {
                    double companyShare = (subsidyPercentage / 100) * price;
                    double employeeShare = price - companyShare;
                    totalCompanyPaid += companyShare;
                    totalEmployeePaid += employeeShare;
                }
                else if (orderCode.ToString().EndsWith("C")) // Company pays fully
                {
                    totalCompanyPaid += price;
                }
            }

            // Generate response
            // Return structured report
            return new DailyCompanyReport
            {
                Date = $"{day}/{month}/{year}",
                CompanyId = companyId,
                ItemWisePlateCount = itemWisePlateCount,
                TotalBill = totalBill,
                TotalCompanyPaid = totalCompanyPaid,
                TotalEmployeePaid = totalEmployeePaid
            };

        }

    }
}
