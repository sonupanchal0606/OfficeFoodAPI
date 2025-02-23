using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeFoodAPI.Data;
using OfficeFoodAPI.Model;

namespace OfficeFoodAPI.Handlers
{
    public class VendorHandler
    {
        private FoodDbContext _context;

        public VendorHandler(FoodDbContext context)
        {
            _context = context;
        }
        public async Task<List<Vendor>> GetVendors()
        {
            var list = await _context.vendor_mstr.Include(m => m.menu_item).ToListAsync();
            if (list == null)
            {
                throw new Exception("No records found");
            }
            return list;
        }

        public async Task<Vendor> GetVendorById(Guid id)
        {
            var record = await _context.vendor_mstr.Where(u => u.vendorid == id).Include(m => m.menu_item).FirstOrDefaultAsync();
            if (record == null)
            {
                throw new Exception("No records found");
            }

            return record;
        }


        public async Task<Vendor> PostVendor(Vendor_post value)
        {
            Vendor vendor = new Vendor()
            {
                name = value.name,
                serviceareas = value.serviceareas,
                menu_item = value.menu_item,
                createdat = DateTime.UtcNow,
                upatedat = DateTime.UtcNow,
            };
            await _context.vendor_mstr.AddAsync(vendor);
            await _context.SaveChangesAsync();

            var record = await _context.vendor_mstr.Where(u => u.vendorid == vendor.vendorid).FirstOrDefaultAsync();
            if (record == null)
            {
                throw new Exception("Error in saving records");
            }
            return record;
        }


        public async Task<Vendor> PutVendor(Guid id, Vendor_post value)
        {
            var data = await _context.vendor_mstr.Where(c => c.vendorid == id).Include(m => m.menu_item).FirstOrDefaultAsync();
            if (data == null)
            {
                throw new Exception("No record to update");
            }
            if (value.name != null)
            {
                data.name = value.name;
            }
            if (value.serviceareas != null)
            {
                data.serviceareas = value.serviceareas;
            }
            if (value.menu_item != null) 
            {
                data.menu_item = value.menu_item;
            }

            data.upatedat = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return data;
        }

        public async Task<JsonResult> DeleteVendor(Guid id)
        {
            var record = await _context.vendor_mstr.Where(u => u.vendorid == id).FirstOrDefaultAsync();
            _context.vendor_mstr.Remove(record);
            await _context.SaveChangesAsync();

            return new JsonResult("Vendor deleted :" + record.name);
        }

        
        public async Task<List<Company>> GetCompaniesServedByVendor(Guid id)
        {
            var record = await _context.company_mstr.Where(u => u.vendorid == id).AsNoTracking().ToListAsync();
            return record; 
        }

        public async Task<MonthlyReport> GetMonthlyReport(Guid vendorId, Guid companyId, int month, int year)
        {
            // Step 1: Fetch Employee Orders for the specified company and vendor
            var orders = await _context.employee_history_mstr
                .Where(e => e.companyid == companyId &&
                            e.company.vendorid == vendorId &&
                            e.month == month &&
                            e.year == year)
                .ToListAsync();

            // Step 2: Fetch Menu Item Prices (Dictionary: { itemname -> price })
            var menuItems = await _context.menuitem_mstr.ToDictionaryAsync(m => m.itemname, m => m.price);

            // Step 3: Initialize report data
            double totalMonthlyRevenue = 0;
            int totalPlates = 0;
            Dictionary<string, double> itemWiseTotal = new();
            Dictionary<int, double> dayWiseTotal = new();

            // Step 4: Aggregate Data
            foreach (var order in orders)
            {
                for (int day = 1; day <= 31; day++)
                {
                    var orderProperty = typeof(EmployeeOrderHistory).GetProperty($"day{day}");
                    if (orderProperty == null) continue;

                    var orderCode = (OrderCode)orderProperty.GetValue(order);
                    if (orderCode == OrderCode.NoOrder || orderCode == OrderCode.OrderCancelled) continue;

                    // Determine item companyname from OrderCode (Updated to return string)
                    string itemName = GetItemNameFromOrderCode(orderCode);
                    if (string.IsNullOrEmpty(itemName)) continue;

                    // Ensure dictionary entry exists
                    if (!itemWiseTotal.ContainsKey(itemName)) itemWiseTotal[itemName] = 0;

                    // Fetch price using the item companyname
                    double itemPrice = menuItems.TryGetValue(itemName, out var price) ? price : 0;
                    itemWiseTotal[itemName] += itemPrice;

                    // Update monthly total
                    totalMonthlyRevenue += itemPrice;
                    totalPlates++;

                    // Update day-wise total
                    if (!dayWiseTotal.ContainsKey(day)) dayWiseTotal[day] = 0;
                    dayWiseTotal[day] += itemPrice;
                }
            }

            // Step 5: Return Report
            return new MonthlyReport
            {
                TotalMonthlyRevenue = totalMonthlyRevenue,
                ItemWiseTotal = itemWiseTotal,
                DayWiseTotal = dayWiseTotal,
                TotalPlates = totalPlates
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

        public async Task<CompanyMonthlyReportSummary> GetMonthlyReportOfCompanies(Guid vendorId, int year, int month)
        {
            // Step 1: Fetch all companies under the vendor
            var companies = await _context.company_mstr
                .Where(c => c.vendorid == vendorId)
                .Include(v => v.vendor)
                .ToListAsync();

            if (!companies.Any()) return null;

            double totalOverallRevenue = 0;
            List<CompanyMonthlyReport> companyReports = new();

            foreach (var company in companies)
            {
                // Step 2: Fetch all employee orders for the company
                var orders = await _context.employee_history_mstr
                    .Where(e => e.companyid == company.companyid &&
                                e.company.vendorid == vendorId &&
                                e.month == month &&
                                e.year == year)
                    .ToListAsync();

                if (!orders.Any()) continue;

                // Step 3: Fetch menu item prices dynamically
                var menuPrices = await _context.menuitem_mstr.ToDictionaryAsync(m => m.itemname, m => m.price);

                int totalPlates = 0;
                double totalCompanyBill = 0;

                // Step 4: Aggregate Data
                foreach (var order in orders)
                {
                    for (int day = 1; day <= 31; day++)
                    {
                        var orderProperty = typeof(EmployeeOrderHistory).GetProperty($"day{day}");
                        if (orderProperty == null) continue;

                        var orderCode = (OrderCode)orderProperty.GetValue(order);
                        if (orderCode == OrderCode.NoOrder || orderCode == OrderCode.OrderCancelled) continue;

                        // Get item companyname from orderCode
                        string itemName = GetItemNameFromOrderCode(orderCode);
                        if (!string.IsNullOrEmpty(itemName) && menuPrices.TryGetValue(itemName, out double price))
                        {
                            totalCompanyBill += price;
                            totalPlates++;
                        }
                    }
                }

                // Update overall revenue
                totalOverallRevenue += totalCompanyBill;

                // Add to report list
                companyReports.Add(new CompanyMonthlyReport
                {
                    CompanyName = company.companyname,
                    TotalPlates = totalPlates,
                    TotalBill = totalCompanyBill
                });
            }

            return new CompanyMonthlyReportSummary
            {
                Companies = companyReports,
                TotalOverallMonthlyRevenue = totalOverallRevenue
            };
        }

        public async Task<DateWiseReport> GetDateWiseReport(Guid companyId, int day, Guid vendorId, int month, int year)
        {
            // Step 1: Fetch orders for the given company, vendor, and date
            var orders = await _context.employee_history_mstr
                .Where(e => e.companyid == companyId &&
                            e.company.vendorid == vendorId &&
                            e.month == month &&
                            e.year == year)
                .ToListAsync();

            if (!orders.Any()) return null;

            // Step 2: Fetch menu item prices dynamically
            var menuPrices = await _context.menuitem_mstr.ToDictionaryAsync(m => m.itemname, m => m.price);

            int totalPlates = 0;
            double totalRevenue = 0;
            Dictionary<string, int> itemWiseCounts = new();

            // Step 3: Aggregate Data for the given day
            foreach (var order in orders)
            {
                var orderProperty = typeof(EmployeeOrderHistory).GetProperty($"day{day}");
                if (orderProperty == null) continue;

                var orderCode = (OrderCode)orderProperty.GetValue(order);
                if (orderCode == OrderCode.NoOrder || orderCode == OrderCode.OrderCancelled) continue;

                // Get item name from OrderCode
                string itemName = GetItemNameFromOrderCode(orderCode);
                if (!string.IsNullOrEmpty(itemName))
                {
                    if (!itemWiseCounts.ContainsKey(itemName))
                        itemWiseCounts[itemName] = 0;

                    itemWiseCounts[itemName]++;

                    if (menuPrices.TryGetValue(itemName, out double price))
                    {
                        totalRevenue += price;
                        totalPlates++;
                    }
                }
            }

            // Step 4: Return Report
            return new DateWiseReport
            {
                CompanyId = companyId,
                VendorId = vendorId,
                Day = day,
                Month = month,
                Year = year,
                TotalPlates = totalPlates,
                TotalRevenue = totalRevenue,
                ItemWiseCounts = itemWiseCounts
            };
        }




    }
}
