using Khazen.Application.Common.Interfaces;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Globalization;

namespace Khazen.Application.Common.Services
{
    public class PayslipGenerator : IPayslipGenerator
    {
        public byte[] GeneratePdf(Salary salary)
        {
            using var doc = new PdfDocument();
            var page = doc.AddPage();
            page.Size = PdfSharpCore.PageSize.A4;
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Verdana", 12, XFontStyle.Regular);

            double y = 40;
            void DrawLine(string label, string value)
            {
                gfx.DrawString(label, font, XBrushes.Black, new XRect(40, y, 200, 20), XStringFormats.TopLeft);
                gfx.DrawString(value, font, XBrushes.Black, new XRect(250, y, 300, 20), XStringFormats.TopLeft);
                y += 25;
            }

            gfx.DrawString("Payslip", new XFont("Verdana", 18, XFontStyle.Bold), XBrushes.Black, new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
            y += 40;

            DrawLine("Employee ID:", salary.EmployeeId.ToString());
            DrawLine("Salary Date:", salary.SalaryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            if (salary.Employee != null)
                DrawLine("Employee Name:", $"{salary.Employee.FirstName} {salary.Employee.LastName}");

            y += 10;
            gfx.DrawLine(XPens.Black, 40, y, page.Width - 40, y);
            y += 15;

            DrawLine("Basic Salary:", salary.BasicSalary.ToString("C"));
            DrawLine("Bonus:", salary.TotalBonus.ToString("C"));
            DrawLine("Deduction:", salary.TotalDeduction.ToString("C"));
            DrawLine("Advance:", salary.TotalAdvance.ToString("C"));
            DrawLine("Net Salary:", salary.TotalAdvance.ToString("C"));

            y += 10;
            gfx.DrawLine(XPens.Black, 40, y, page.Width - 40, y);
            y += 15;

            DrawLine("Notes:", string.IsNullOrWhiteSpace(salary.Notes) ? "-" : salary.Notes);

            using var ms = new MemoryStream();
            doc.Save(ms, false);
            return ms.ToArray();
        }
    }
}
