using Microsoft.EntityFrameworkCore;
using Npgsql.Internal;
using OfficeFoodAPI.Data;
using OfficeFoodAPI.Model;
using System.Reflection.Metadata;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Document = iTextSharp.text.Document;

namespace OfficeFoodAPI.Handlers
{
    public class EmployeeOrderHistoryHandler
    {
        private FoodDbContext _context;

        public EmployeeOrderHistoryHandler(FoodDbContext context)
        {
            _context = context;
        }

        /*        public async Task<bool> PatchEmployee(Guid employeeId, JsonPatchDocument<EmployeeOrderHistory> patchDocument)
                {
                    var employee = await _context.employee_history_mstr.FindAsync(employeeId);
                    if (employee == null) return false;

                    patchDocument.ApplyTo(employee);
                    await _context.SaveChangesAsync();
                    return true;
                }*/

        public async Task<string> SubscribePlate(PlateSubscriptionRequest request)
        {
            var employee = await _context.employee_history_mstr
                .Where(e => e.userid == request.userid)
                .FirstOrDefaultAsync();
            if (employee == null)
            {
                throw new Exception("Employee history not found.");
                // now create new entry for the future date instead of throwing exception
            }

            var menuItem = await _context.menuitem_mstr.FindAsync(request.ItemId);
            if (menuItem == null)
            {
                throw new Exception("Menu Item not found.");
            }

            var vendorMenu = await _context.company_mstr
                .Where(vm => vm.vendorid == request.VendorId && vm.companyid == request.CompanyId)
                .FirstOrDefaultAsync();

            if (vendorMenu == null)
            {
                throw new Exception("Vendor not assigned to company not found.");
            }

            // Generate order entries for each day in the subscription period
            int startDate = request.StartDate.Day;
            int endDate = request.EndDate.Day;
            var orderHistory = await _context.employee_history_mstr
                     .Where(e => e.userid == request.userid & e.month == request.StartDate.Month && e.year == request.StartDate.Year)
                     .FirstOrDefaultAsync();

            if (orderHistory == null)
            {
                orderHistory = new EmployeeOrderHistory
                {
                    userid = request.userid,
                    companyid = employee.companyid,
                    year = request.StartDate.Month,
                    month = request.StartDate.Year,
                    createdat = DateTime.UtcNow,
                    upatedat = DateTime.UtcNow
                };
                _context.employee_history_mstr.Add(orderHistory);
            }

            // var daysInMonth = DateTime.DaysInMonth(year, month);
            OrderCode orderCode = await GetOrderCode(request.ItemId, request.IsChargedToCompany);
            for (int day = startDate; day <= endDate; day++)
            {
 /*               DateTime currentDate = new DateTime(year, month, day);
                if (currentDate < employeeHistory.StartDate || currentDate > employeeHistory.EndDate)
                {
                    SetOrderCode(orderHistory, day, OrderCode.NoOrder);
                    continue;
                }*/

                
                SetOrderCode(orderHistory, day, orderCode);
            }

            orderHistory.upatedat = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return ("subscribed");
        }

        public async Task<OrderCode> GetOrderCode(Guid itemId, bool IsCompanyCharged)
        {
            var menuItem = await _context.menuitem_mstr.FindAsync(itemId);

            if (menuItem == null)
            {
                throw new Exception("Menu Item not found.");
            }

            return menuItem.itemname.ToLower() switch
            {
                "Veg" => IsCompanyCharged ? OrderCode.SVC : OrderCode.SVE,
                "Non Veg" => IsCompanyCharged ? OrderCode.SNC : OrderCode.SNE,
                "Veg Special" => IsCompanyCharged ? OrderCode.SVsC : OrderCode.SVsE,
                "Non Veg Special" => IsCompanyCharged ? OrderCode.SNsC : OrderCode.SNsE,
                _ => OrderCode.NoOrder
            };


        }



        private void SetOrderCode(EmployeeOrderHistory orderHistory, int day, OrderCode code)
        {
            var property = typeof(EmployeeOrderHistory).GetProperty($"day{day}");
            if (property != null)
            {
                property.SetValue(orderHistory, code);
            }
        }

        public async Task<object> PlaceOrderAsync(PlaceOrderRequest request)
        {
            var menuItem = await _context.menuitem_mstr.FindAsync(request.ItemId);
            if (menuItem == null)
            {
                throw new Exception("Menu Item not found.");
            }

            var orderHistory = await _context.employee_history_mstr
                .FirstOrDefaultAsync(e => e.employeeid == request.UserId && e.year == DateTime.UtcNow.Year && e.month == DateTime.UtcNow.Month);

/*            if (orderHistory == null)
            {
                orderHistory = new EmployeeOrderHistory
                {
                    employeeid = request.EmployeeId,
                    companyid = request.CompanyId,
                    userid = request.UserId,
                    year = DateTime.UtcNow.Year,
                    month = DateTime.UtcNow.Month,
                    createdat = DateTime.UtcNow,
                    upatedat = DateTime.UtcNow
                };
                _context.employee_order_history.Add(orderHistory);
            }*/

            int today = DateTime.UtcNow.Day;
            OrderCode orderCode = MapItemNameToOrderCode(menuItem.itemname);

            typeof(EmployeeOrderHistory).GetProperty($"day{today}")?.SetValue(orderHistory, orderCode);
            orderHistory.upatedat = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new { Message = "Order placed successfully", OrderCode = orderCode };
        }

        public async Task<object> CancelOrderAsync(CancelOrderRequest request)
        {
            var orderHistory = await _context.employee_history_mstr
                .FirstOrDefaultAsync(e => e.employeeid == request.UserId && e.year == DateTime.UtcNow.Year && e.month == DateTime.UtcNow.Month);

            if (orderHistory == null)
            {
                throw new Exception("No order history found for this employee.");
            }

            int today = DateTime.UtcNow.Day;
            typeof(EmployeeOrderHistory).GetProperty($"day{today}")?.SetValue(orderHistory, OrderCode.OrderCancelled);
            orderHistory.upatedat = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new { Message = "Order canceled successfully" };
        }

        private OrderCode MapItemNameToOrderCode(string itemName)
        {
            return itemName.ToLower() switch
            {
                "veg thali" => OrderCode.UVC,
                "non veg thali" => OrderCode.UNC,
                "special thali veg" => OrderCode.UVsC,
                "special thali non veg" => OrderCode.UNsC,
                _ => OrderCode.NoOrder
            };
        }
        public async Task<MonthlyReportDto> GenerateMonthlyReportAsync(int month, int year, Guid userId)
        {
            var orderHistory = await _context.employee_history_mstr
                .Where(e => e.userid == userId && e.month == month && e.year == year)
                .FirstOrDefaultAsync();

            double compnay_share = await _context.company_mstr
                .Where(c => c.companyid == orderHistory.companyid)
                .Select(c => c.subsidyperplate)
                .FirstOrDefaultAsync();

            double employee_share = 100 - compnay_share;

            if (orderHistory == null)
            {
                throw new Exception("No order history found for this user.");
            }

            var menuItemList = await _context.menuitem_mstr.ToListAsync();
            if (menuItemList == null)
            {
                throw new Exception("Menu Item not found.");
            }

            List<DailyOrderDetails> dailyOrders = new();
            double totalBill = 0;
            int totalPlates = 0;
            double employeePaid = 0;
            double companyPaid = 0;

            for (int day = 1; day <= DateTime.DaysInMonth(year, month); day++)
            {
                var orderCode = (OrderCode)typeof(EmployeeOrderHistory).GetProperty($"day{day}")?.GetValue(orderHistory);

                if (orderCode != OrderCode.NoOrder && orderCode != OrderCode.OrderCancelled)
                {
                    var menuItem = MapOrderCodeToMenuItem(orderCode); // need to fix this.. its not correct mapping
                    if (menuItem == null) continue;

                    totalBill += menuItem.price;
                    totalPlates++;

                    double companyShare = (menuItem.price * compnay_share) / 100; // 70% company paid
                    double employeeShare = (menuItem.price * employee_share) / 100; // 30% employee paid

                    companyPaid += companyShare;
                    employeePaid += employeeShare;

                    dailyOrders.Add(new DailyOrderDetails
                    {
                        Date = new DateTime(year, month, day),
                        FoodItem = menuItem.itemname,
                        Price = menuItem.price,
                        EmployeePaid = employeeShare,
                        CompanyPaid = companyShare
                    });
                }
            }

            return new MonthlyReportDto
            {
                UserId = userId,
                Month = month,
                Year = year,
                DailyOrders = dailyOrders,
                TotalPlates = totalPlates,
                TotalBill = totalBill,
                EmployeePaidPercentage = (employeePaid / totalBill) * 100,
                CompanyPaidPercentage = (companyPaid / totalBill) * 100
            };
        }

        public async Task<byte[]> GenerateMonthlyReportPdfAsync(int month, int year, Guid userId)
        {
            var report = await GenerateMonthlyReportAsync(month, year, userId);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document();
                PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                document.Add(new Paragraph($"Monthly Report - {year}/{month}"));
                document.Add(new Paragraph($"User ID: {report.UserId}"));
                document.Add(new Paragraph($"Total Plates: {report.TotalPlates}"));
                document.Add(new Paragraph($"Total Bill: ₹{report.TotalBill}"));
                document.Add(new Paragraph($"Employee Paid: {report.EmployeePaidPercentage:F2}%"));
                document.Add(new Paragraph($"Company Paid: {report.CompanyPaidPercentage:F2}%"));

                PdfPTable table = new PdfPTable(4);
                table.AddCell("Date");
                table.AddCell("Food Item");
                table.AddCell("Price");
                table.AddCell("Employee Paid");

                foreach (var order in report.DailyOrders)
                {
                    table.AddCell(order.Date.ToString("yyyy-MM-dd"));
                    table.AddCell(order.FoodItem);
                    table.AddCell($"₹{order.Price}");
                    table.AddCell($"₹{order.EmployeePaid}");
                }

                document.Add(table);
                document.Close();

                return memoryStream.ToArray();
            }
        }

        private MenuItem MapOrderCodeToMenuItem(OrderCode orderCode)
        {
            return _context.menuitem_mstr.FirstOrDefault(m => m.itemname == orderCode.ToString());
        }
    }
}
