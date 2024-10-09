using System.Reflection.Metadata;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestingSoftwareAPI.Data;
using TestingSoftwareAPI.Models;
using TestingSoftwareAPI.Models.Process;
using TestingSoftwareAPI.Models.Queries;
using TestingSoftwareAPI.Models.ViewModel;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Barcodes;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Colors;
using iText.IO.Font.Constants;
using System.Data.Common;
using OfficeOpenXml;

namespace TestingSoftwareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationCodeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly StudentExRegisCodeQuery _studentQuery;
        RandomValue rdValue = new RandomValue();
        public RegistrationCodeController(ApplicationDbContext context, StudentExRegisCodeQuery studentExRegisCodeQuery)
        {
            _context = context;
            _studentQuery = studentExRegisCodeQuery;
        }

        // GET: api/RegistrationCode
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegistrationCode>>> GetRegistrationCode()
        {
            return await _context.RegistrationCode.ToListAsync();
        }

        // GET: api/RegistrationCode/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RegistrationCode>> GetRegistrationCode(Guid id)
        {
            var registrationCode = await _context.RegistrationCode.FindAsync(id);

            if (registrationCode == null)
            {
                return NotFound();
            }

            return registrationCode;
        }
        [HttpGet("count/{id}")]
        public async Task<ActionResult<RegistrationCode>> CountRegistrationCode(int id)
        {
            var countRegistrationCode = await _context.RegistrationCode.Where(m => m.ExamId == id).CountAsync();
            return Ok(countRegistrationCode);
        }

        // PUT: api/RegistrationCode/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRegistrationCode(Guid id, RegistrationCode registrationCode)
        {
            if (id != registrationCode.RegistrationCodeID)
            {
                return BadRequest();
            }

            _context.Entry(registrationCode).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegistrationCodeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [HttpPut("code-generation")]
        public async Task<IActionResult> RegistrationCodeGeneration(int examId, bool isOverGenRegCode)
        {
            var exam = await _context.Exam.FindAsync(examId);
            if (exam == null)
            {
                return NotFound();
            }
            else if(exam.Status == true) return Ok("Kỳ thi đã bị khoá, vui lòng liên hệ quản trị viên!");
            else {
                var studentExams = _studentQuery.GetStudentExamByExamID(examId);
                if(studentExams.Result.Count==0) {
                    return Ok("Cần nhập danh sách thí sinh dự thi trước khi sinh phách!");
                }
                else {
                    if(isOverGenRegCode) {
                        await _context.BulkDeleteAsync(await _context.RegistrationCode.Where(m => m.ExamId == examId).ToListAsync());
                    }
                    var list = _studentQuery.getListUnregisteredCodeByExamID(examId);
                    await _context.RegistrationCode.AddRangeAsync (list);
                    await _context.BulkSaveChangesAsync();
                    
                    return Ok("Sinh thành công " + list.Count + " phách");
                }
            }
        }

        // POST: api/RegistrationCode
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RegistrationCode>> PostRegistrationCode(RegistrationCode registrationCode)
        {
            _context.RegistrationCode.Add(registrationCode);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRegistrationCode", new { id = registrationCode.RegistrationCodeID }, registrationCode);
        }

        // DELETE: api/RegistrationCode/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegistrationCode(Guid id)
        {
            var registrationCode = await _context.RegistrationCode.FindAsync(id);
            if (registrationCode == null)
            {
                return NotFound();
            }

            _context.RegistrationCode.Remove(registrationCode);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpGet("download-excel-file")]
        public async Task<IActionResult> DownloadExcel(int examID, int examBag)
        {
            // Truy vấn dữ liệu từ cơ sở dữ liệu
            var registrationCodeNumbers = await (from regisCode in _context.RegistrationCode
                                                join stdExam in _context.StudentExam on regisCode.StudentExamID equals stdExam.StudentExamID
                                                where stdExam.ExamId == examID && stdExam.ExamBag == examBag
                                                select regisCode.RegistrationCodeNumber).ToListAsync();
            var listVT = await (from regisCode in _context.RegistrationCode
                                                join stdExam in _context.StudentExam on regisCode.StudentExamID equals stdExam.StudentExamID
                                                where stdExam.ExamId == examID && stdExam.ExamBag == examBag && stdExam.IsActive == false
                                                select regisCode.RegistrationCodeNumber).ToListAsync();
            var listDT = await (from regisCode in _context.RegistrationCode
                                                join stdExam in _context.StudentExam on regisCode.StudentExamID equals stdExam.StudentExamID
                                                where stdExam.ExamId == examID && stdExam.ExamBag == examBag && stdExam.IsActive == true
                                                select regisCode.RegistrationCodeNumber).ToListAsync();

            // Tạo một workbook Excel mới
            using (var package = new ExcelPackage())
            {
                var worksheet1 = package.Workbook.Worksheets.Add("sheet1");
                worksheet1.Cells["A1"].Value = "Phach";
                worksheet1.Cells["B1"].Value = "Diem1";
                worksheet1.Cells["C1"].Value = "Diem2";
                worksheet1.Cells["D1"].Value = "DiemTB";
                worksheet1.Cells["A2"].LoadFromCollection(registrationCodeNumbers, true);
                var headerRange1 = worksheet1.Cells["A1:D1"];
                var dataRange1 = worksheet1.Cells["A1:D" + (registrationCodeNumbers.Count + 1)];
                dataRange1.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                dataRange1.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                dataRange1.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                dataRange1.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                headerRange1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                dataRange1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                
                var worksheet2 = package.Workbook.Worksheets.Add("sheet2");
                worksheet2.Cells["A1"].Value = "Phach";
                worksheet2.Cells["B1"].Value = "Diem1";
                worksheet2.Cells["C1"].Value = "Diem2";
                worksheet2.Cells["D1"].Value = "DiemTB";
                worksheet2.Cells["A2"].LoadFromCollection(listDT, true);
                var headerRange2 = worksheet2.Cells["A1:D1"];
                var dataRange2 = worksheet2.Cells["A1:D" + (listDT.Count + 1)];
                dataRange2.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                dataRange2.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                dataRange2.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                dataRange2.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                headerRange2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                dataRange2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                var worksheet3 = package.Workbook.Worksheets.Add("sheet3");
                worksheet3.Cells["A1"].Value = "Phach";
                worksheet3.Cells["B1"].Value = "Diem1";
                worksheet3.Cells["C1"].Value = "Diem2";
                worksheet3.Cells["D1"].Value = "DiemTB";
                worksheet3.Cells["A2"].LoadFromCollection(listVT, true);
                var headerRange3 = worksheet3.Cells["A1:D1"];
                var dataRange3 = worksheet3.Cells["A1:D" + (listVT.Count + 1)];
                dataRange3.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                dataRange3.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                dataRange3.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                dataRange3.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                headerRange3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                dataRange3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                // Tạo một stream để lưu trữ dữ liệu của tệp Excel
                var stream = new MemoryStream();
                
                // Lưu workbook vào stream
                package.SaveAs(stream);

                // Thiết lập vị trí của stream về đầu
                stream.Position = 0;

                // Trả về một file tải xuống có tên "RegistrationCodes.xlsx" và kiểu dữ liệu là "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RegistrationCodes.xlsx");
            }
        }
        [HttpGet("tai-file-phach")]
        public IActionResult TaiFile(int examBag, int examId)
        {
            var result = (from stdEx in _context.StudentExam
                          join std in _context.Student
                              on stdEx.StudentCode equals std.StudentCode
                          join regCode in _context.RegistrationCode
                              on stdEx.StudentExamID equals regCode.StudentExamID
                          join subject in _context.Subject
                              on stdEx.SubjectCode equals subject.SubjectCode
                          join exam in _context.Exam
                              on stdEx.ExamId equals exam.ExamId
                          where stdEx.ExamId == examId && stdEx.ExamBag == examBag
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
            using (MemoryStream stream = new MemoryStream())
            {
                PdfFont font = PdfFontFactory.CreateFont("Uploads/fonts/Arial.ttf", PdfEncodings.IDENTITY_H);
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                pdf.SetDefaultPageSize(PageSize.A4);
                var document = new iText.Layout.Document(pdf, PageSize.A4.Rotate());
                document.SetMargins(10, 10, 10, 10);
                PdfFont f = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
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
                foreach (var student in result)
                {
                    Table currentMainColumn = mainColumns[count % 1];
                    AddColumnToTable(currentMainColumn, student, font, pdf, count + 1);
                    count += 1;

                }
                for (int i = 0; i < mainColumns.Count; i++)
                {
                    document.Add(mainColumns[i]);
                }

                document.Close();
                return File(stream.ToArray(), "application/pdf", "Students.pdf");
            }
        }        
        private void AddColumnToTable(Table currentMainColumn, RegistrationCodeListVM student, PdfFont font, PdfDocument pdf, int stt)
        {
            Table currentSmallColumn = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            float columnWidth = (float)(PageSize.A4.GetWidth() / 4.0);
            float columnHeight = (float)(PageSize.A4.GetHeight() / 32.0);
            Cell studentCodeCell = new Cell().SetWidth(columnWidth).SetHeight(columnHeight).SetBorder(Border.NO_BORDER).Add(new Paragraph($"{stt}-{student.StudentCode} - {student.RegistrationCodeNumber}\n {student.LastName} {student.FirstName}")).SetFont(font)
                                                .SetFontSize(8).SetTextAlignment(TextAlignment.CENTER);
            currentSmallColumn.AddCell(studentCodeCell);

            Cell registrationCodeCell = new Cell().SetWidth(columnWidth).SetHeight(columnHeight).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.CENTER);
            
            BarcodeQRCode qrCode = new BarcodeQRCode(student.RegistrationCodeNumber.ToString());
            PdfFormXObject qrCodeObject = qrCode.CreateFormXObject(ColorConstants.BLACK, pdf);
            Image qrCodeImage = new Image(qrCodeObject);
            qrCodeImage.SetWidth(20);
            qrCodeImage.SetHeight(20);
            Paragraph paragraph1 = new Paragraph();
            paragraph1.Add(qrCodeImage);
            paragraph1.Add(new Text($"{student.RegistrationCodeNumber}")).SetFont(font);
            registrationCodeCell.Add(paragraph1);
            currentSmallColumn.AddCell(registrationCodeCell);

            currentMainColumn.AddCell(new Cell().Add(currentSmallColumn));
        }
        private bool RegistrationCodeExists(Guid id)
        {
            return _context.RegistrationCode.Any(e => e.RegistrationCodeID == id);
        }
    }
}
