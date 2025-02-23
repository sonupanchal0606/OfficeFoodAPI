using OfficeFoodAPI.Model;
using OfficeOpenXml;

namespace OfficeFoodAPI.HelperClasses
{
    public static class ExcelHelper
    {
        public static byte[] GenerateMonthlyEmployeeReport(MonthlyEmployeeReport report)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Monthly Employee Report");

            // Headers
            worksheet.Cells[1, 1].Value = "Employee ID";
            worksheet.Cells[1, 2].Value = "Employee Name";
            worksheet.Cells[1, 3].Value = "Total Plates";
            worksheet.Cells[1, 4].Value = "Total Bill";
            worksheet.Cells[1, 5].Value = "Company Paid";
            worksheet.Cells[1, 6].Value = "Employee Paid";

            int row = 2;
            foreach (var item in report.EmployeeRecords)
            {
                worksheet.Cells[row, 1].Value = item.EmployeeId.ToString();
                worksheet.Cells[row, 2].Value = item.EmployeeName;
                worksheet.Cells[row, 3].Value = item.TotalPlates;
                worksheet.Cells[row, 4].Value = item.TotalBill;
                worksheet.Cells[row, 5].Value = item.CompanyPaid;
                worksheet.Cells[row, 6].Value = item.EmployeePaid;
                row++;
            }

            // Summary row
            worksheet.Cells[row + 1, 2].Value = "TOTAL:";
            worksheet.Cells[row + 1, 3].Value = report.TotalPlates;
            worksheet.Cells[row + 1, 4].Value = report.TotalBill;
            worksheet.Cells[row + 1, 5].Value = report.TotalCompanyPaid;
            worksheet.Cells[row + 1, 6].Value = report.TotalEmployeePaid;

            return package.GetAsByteArray();
        }
    }

}
