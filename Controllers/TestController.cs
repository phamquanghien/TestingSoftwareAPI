using iText.Barcodes;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using TestingSoftwareAPI.Data;
using TestingSoftwareAPI.Models.ViewModel;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using TestingSoftwareAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TestingSoftwareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Test : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public Test(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("admin-get-list")]
        public async Task<ActionResult<IEnumerable<StudentExam>>> AdminGetList(int examBag = 124)
        {
            var model = await _context.StudentExam.Where(m => m.ExamBag == 0 || m.ExamBag == examBag).Include(m => m.Student).ToListAsync();
            return model;
        }
        [HttpGet("get-list")]
        public async Task<ActionResult<IEnumerable<RegistrationCodeListVM>>> GetList(int examId, int examBag)
        {
            var query = await (from stdEx in _context.StudentExam
                         join std in _context.Student on stdEx.StudentCode equals std.StudentCode
                         join regCode in _context.RegistrationCode on stdEx.StudentExamID equals regCode.StudentExamID
                         join subject in _context.Subject on stdEx.SubjectCode equals subject.SubjectCode
                         join exam in _context.Exam on stdEx.ExamId equals exam.ExamId
                         where stdEx.ExamId == examId && (examBag == 0 || stdEx.ExamBag == examBag)
                         orderby stdEx.ExamBag
                         select new RegistrationCodeListVM
                         {
                             RegistrationCodeNumber = regCode.RegistrationCodeNumber,
                             StudentCode = stdEx.StudentCode,
                             LastName = std.LastName,
                             FirstName = std.FirstName,
                             SubjectName = subject.SubjectName,
                             ExamBag = stdEx.ExamBag,
                             ExamCode = exam.ExamCode
                         }).ToListAsync();
            return query;
        }
        [HttpGet("down-load-registration-code-file")]
        public async Task<IActionResult> DownloadRegistrionCodeFile(int examId, int examBag)
        {
            var query = await (from stdEx in _context.StudentExam
                         join std in _context.Student on stdEx.StudentCode equals std.StudentCode
                         join regCode in _context.RegistrationCode on stdEx.StudentExamID equals regCode.StudentExamID
                         join subject in _context.Subject on stdEx.SubjectCode equals subject.SubjectCode
                         join exam in _context.Exam on stdEx.ExamId equals exam.ExamId
                         where stdEx.ExamId == examId && (examBag == 0 || stdEx.ExamBag == examBag)
                         orderby stdEx.ExamBag
                         select new RegistrationCodeListVM
                         {
                             RegistrationCodeNumber = regCode.RegistrationCodeNumber,
                             StudentCode = stdEx.StudentCode,
                             LastName = std.LastName,
                             FirstName = std.FirstName,
                             SubjectName = subject.SubjectName,
                             ExamBag = stdEx.ExamBag,
                             ExamCode = exam.ExamCode
                         }).ToListAsync();
            var ExambagList = query.Select(e => e.ExamBag).Distinct().ToList();
            using (MemoryStream stream = new MemoryStream())
            {
                Stopwatch stoppoint = new Stopwatch();
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                pdf.SetDefaultPageSize(PageSize.A4);
                var document = new iText.Layout.Document(pdf, PageSize.A4.Rotate());

                foreach (var exambagitem in ExambagList)
                {
                    document.SetMargins(10, 10, 10, 10);
                    var SubjectName = query.Where(e => e.ExamBag == exambagitem).Select(e => e.SubjectName).FirstOrDefault();
                    var ExamCode = query.Where(e => e.ExamBag == exambagitem).Select(e => e.ExamCode).FirstOrDefault();
                    var fontFile = "Uploads/fonts/Arial.ttf";
                    int page = (int)Math.Ceiling((float)((query.Where(e => e.ExamBag == exambagitem).Count() / 32) + 0.4));

                    var baseFont = PdfFontFactory.CreateFont(fontFile, PdfEncodings.IDENTITY_H);
                    Table table = new Table(3);
                    table.SetWidth(800);
                    table.SetHeight(50);
                    table.SetHorizontalAlignment(HorizontalAlignment.CENTER);

                    Paragraph headerLeft = new Paragraph(ExamCode)
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetFontSize(12)
                        .SetFixedLeading(15)
                        .SetFont(baseFont)
                        .SetFontColor(ColorConstants.RED);

                    Paragraph headerMiddle = new Paragraph("TRƯỜNG ĐẠI HỌC MỎ-ĐỊA CHẤT - PHÒNG ĐẢM BẢO CHẤT LƯỢNG GIÁO DỤC\n" + ExamCode + ": BIÊN BẢN SBD-PHÁCH" + "\nTúi số: " + exambagitem + "-" + SubjectName)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(12)
                        .SetFixedLeading(15)
                        .SetMarginLeft(50)
                        .SetFont(baseFont);

                    Paragraph headerRight = new Paragraph("Tài liêu mật: Trang 1/" + page)
                        .SetTextAlignment(TextAlignment.RIGHT)
                        .SetFontSize(12)
                        .SetFixedLeading(15)
                        .SetFont(baseFont)
                        .SetFontColor(ColorConstants.RED);
                    Cell cellLeft = new Cell().Add(headerLeft)
                        .SetBorder(Border.NO_BORDER);

                    Cell cellMiddle = new Cell().Add(headerMiddle)
                        .SetBorder(Border.NO_BORDER);

                    Cell cellRight = new Cell().Add(headerRight)
                        .SetBorder(Border.NO_BORDER);
                    table.AddCell(cellLeft);
                    table.AddCell(cellMiddle);
                    table.AddCell(cellRight);
                    document.Add(table);

                    float columnWidth = (float)(PageSize.A4.GetWidth() / 2.0);
                    float columnHeight = (float)(PageSize.A4.GetHeight() / 30.0);
                    List<Table> mainColumns = new List<Table>();
                    for (int i = 0; i < 2; i++)
                    {
                        Table mainColumn = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
                        mainColumn.SetBorder(Border.NO_BORDER);
                        mainColumns.Add(mainColumn);
                    }
                    int count = 0;
                    foreach (var student in query.Where(e => e.ExamBag == exambagitem))
                    {
                        Table currentMainColumn = mainColumns[count % 1];
                        AddColumnToTable(currentMainColumn, student, baseFont, pdf, count + 1);
                        count += 1;
                    }
                    for (int i = 0; i < mainColumns.Count; i++)
                    {
                        document.Add(mainColumns[i]);
                    }
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                }

                document.Close();
                // stopwatch.Stop();
                // TimeSpan elapsedTime = stopwatch.Elapsed;
                // return Ok(new { message = "File processed successfully", elapsedTime });
                return File(stream.ToArray(), "application/pdf", "Students.pdf");
            }
        }

        [HttpGet("tai-file-phach")]
        public IActionResult TaiFile(int examBag, int examId)
        {
            // Stopwatch stopwatch = new Stopwatch();
            // stopwatch.Start();
            var query = (from stdEx in _context.StudentExam
                         join std in _context.Student on stdEx.StudentCode equals std.StudentCode
                         join regCode in _context.RegistrationCode on stdEx.StudentExamID equals regCode.StudentExamID
                         join subject in _context.Subject on stdEx.SubjectCode equals subject.SubjectCode
                         join exam in _context.Exam on stdEx.ExamId equals exam.ExamId
                         where stdEx.ExamId == examId && (examBag == 0 || stdEx.ExamBag == examBag)
                         orderby stdEx.ExamBag
                         select new RegistrationCodeListVM
                         {
                             RegistrationCodeNumber = regCode.RegistrationCodeNumber,
                             StudentCode = stdEx.StudentCode,
                             LastName = std.LastName,
                             FirstName = std.FirstName,
                             SubjectName = subject.SubjectName,
                             ExamBag = stdEx.ExamBag,
                             ExamCode = exam.ExamCode
                         }).ToList();

            var ExambagList = query.Select(e => e.ExamBag).Distinct().ToList();

            // return Ok(ExambagList);

            using (MemoryStream stream = new MemoryStream())
            {
                Stopwatch stoppoint = new Stopwatch();
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                pdf.SetDefaultPageSize(PageSize.A4);
                var document = new iText.Layout.Document(pdf, PageSize.A4.Rotate());

                foreach (var exambagitem in ExambagList)
                {
                    document.SetMargins(10, 10, 10, 10);
                    var SubjectName = query.Where(e => e.ExamBag == exambagitem).Select(e => e.SubjectName).FirstOrDefault();
                    var ExamCode = query.Where(e => e.ExamBag == exambagitem).Select(e => e.ExamCode).FirstOrDefault();
                    var fontFile = "Uploads/fonts/Arial.ttf";
                    int page = (int)Math.Ceiling((float)((query.Where(e => e.ExamBag == exambagitem).Count() / 32) + 0.4));

                    var baseFont = PdfFontFactory.CreateFont(fontFile, PdfEncodings.IDENTITY_H);
                    Table table = new Table(3);
                    table.SetWidth(800);
                    table.SetHeight(50);
                    table.SetHorizontalAlignment(HorizontalAlignment.CENTER);

                    Paragraph headerLeft = new Paragraph(ExamCode)
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetFontSize(12)
                        .SetFixedLeading(15)
                        .SetFont(baseFont)
                        .SetFontColor(ColorConstants.RED);

                    Paragraph headerMiddle = new Paragraph("TRƯỜNG ĐẠI HỌC MỎ-ĐỊA CHẤT - PHÒNG ĐẢM BẢO CHẤT LƯỢNG GIÁO DỤC\n" + ExamCode + ": BIÊN BẢN SBD-PHÁCH" + "\nTúi số: " + exambagitem + "-" + SubjectName)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(12)
                        .SetFixedLeading(15)
                        .SetMarginLeft(50)
                        .SetFont(baseFont);

                    Paragraph headerRight = new Paragraph("Tài liêu mật: Trang 1/" + page)
                        .SetTextAlignment(TextAlignment.RIGHT)
                        .SetFontSize(12)
                        .SetFixedLeading(15)
                        .SetFont(baseFont)
                        .SetFontColor(ColorConstants.RED);
                    Cell cellLeft = new Cell().Add(headerLeft)
                        .SetBorder(Border.NO_BORDER);

                    Cell cellMiddle = new Cell().Add(headerMiddle)
                        .SetBorder(Border.NO_BORDER);

                    Cell cellRight = new Cell().Add(headerRight)
                        .SetBorder(Border.NO_BORDER);
                    table.AddCell(cellLeft);
                    table.AddCell(cellMiddle);
                    table.AddCell(cellRight);
                    document.Add(table);

                    float columnWidth = (float)(PageSize.A4.GetWidth() / 2.0);
                    float columnHeight = (float)(PageSize.A4.GetHeight() / 30.0);
                    List<Table> mainColumns = new List<Table>();
                    for (int i = 0; i < 2; i++)
                    {
                        Table mainColumn = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
                        mainColumn.SetBorder(Border.NO_BORDER);
                        mainColumns.Add(mainColumn);
                    }
                    int count = 0;
                    foreach (var student in query.Where(e => e.ExamBag == exambagitem))
                    {
                        Table currentMainColumn = mainColumns[count % 1];
                        AddColumnToTable(currentMainColumn, student, baseFont, pdf, count + 1);
                        count += 1;
                    }
                    for (int i = 0; i < mainColumns.Count; i++)
                    {
                        document.Add(mainColumns[i]);
                    }
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                }

                document.Close();
                // stopwatch.Stop();
                // TimeSpan elapsedTime = stopwatch.Elapsed;
                // return Ok(new { message = "File processed successfully", elapsedTime });
                return File(stream.ToArray(), "application/pdf", "Students.pdf");
            }
        }

        private void AddColumnToTable(Table currentMainColumn, RegistrationCodeListVM student, PdfFont font, PdfDocument pdf, int stt)
        {
            float columnWidth = (float)(PageSize.A4.GetWidth() / 4.0);
            float columnHeight = (float)(PageSize.A4.GetHeight() / 32.0);

            // Tạo cột thông tin sinh viên
            Cell studentCell = new Cell().SetWidth(columnWidth).SetHeight(columnHeight).SetBorder(Border.NO_BORDER)
                .Add(new Paragraph($"{stt}-{student.StudentCode} - {student.RegistrationCodeNumber}\n{student.FirstName} {student.LastName} "))
                .SetFont(font).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER);

            // Tạo cột mã QR
            Cell qrCodeCell = new Cell().SetWidth(columnWidth).SetHeight(columnHeight).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.CENTER);

            BarcodeQRCode qrCode = new BarcodeQRCode(student.RegistrationCodeNumber.ToString());
            PdfFormXObject qrCodeObject = qrCode.CreateFormXObject(ColorConstants.BLACK, pdf);
            Image qrCodeImage = new Image(qrCodeObject).SetWidth(20).SetHeight(20);

            qrCodeCell.Add(new Paragraph().Add(qrCodeImage).Add(new Text($"{student.RegistrationCodeNumber}")).SetFont(font));

            // Thêm cột thông tin sinh viên và cột mã QR vào hàng
            Table smallColumn = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth().SetBorder(Border.NO_BORDER);
            smallColumn.AddCell(studentCell);
            smallColumn.AddCell(qrCodeCell);

            // Thêm hàng vào cột chính
            currentMainColumn.AddCell(new Cell().Add(smallColumn));
        }
    }
}