using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TestingSoftwareAPI.Data;
using TestingSoftwareAPI.Models;
using TestingSoftwareAPI.Models.ViewModel;

namespace TestingSoftwareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectExamController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubjectExamController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/SubjectExam
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectExam>>> GetSubjectExams()
        {
            return await _context.SubjectExam.ToListAsync();
        }
        [HttpGet("admin-get-list-by-examId")]
        public async Task<ActionResult<IEnumerable<SubjectExam>>> AminGetSubjectExamByExamID(int examID)
        {
            if (examID == 0 || examID == null)
            {
                return BadRequest("BadRequest");
            }
            var query = await _context.SubjectExam
                    .Include(m => m.Subject) // Bao gồm thông tin của Subject
                    .Where(m => m.ExamId == examID)
                    .ToListAsync();
            if(query == null) {
                return NotFound();
            }
            return query;
        }
        [HttpGet("employee-get-by-exam-bag")]
        public async Task<ActionResult<SubjectExam>> EmployeeGetByExamBag(int examID, int examBag)
        {
            if (examID == 0 || examBag == null)
            {
                return BadRequest("BadRequest");
            }
            
            var subjectExam = await _context.SubjectExam
                    .Include(m => m.Subject) // Bao gồm thông tin của Subject
                    .Where(m => m.ExamId == examID && m.ExamBag == examBag)
                    .FirstOrDefaultAsync();
            if(subjectExam == null) {
                return NotFound();
            }
            return subjectExam;
        }
        [HttpGet("admin-get-by-subject-code")]
        public async Task<ActionResult<List<SubjectExam>>> AdminGetBySubjectCode(int examID, string subjectCode)
        {
            if (examID == 0 || subjectCode == null)
            {
                return BadRequest("BadRequest");
            }
            
            var subjectExam = await _context.SubjectExam
                    .Include(m => m.Subject) // Bao gồm thông tin của Subject
                    .Where(m => m.ExamId == examID && m.SubjectCode.Contains(subjectCode))
                    .ToListAsync();
            if(subjectExam == null) {
                return NotFound();
            }
            return subjectExam;
        }
        [HttpGet("admin-get-by-subject-code-by-examId")]
        public async Task<ActionResult<List<SubjectExam>>> AdminGetBySubjectCode(int examID)
        {
            if (examID == 0)
            {
                return BadRequest("BadRequest");
            }
            
            var subjectExam = await _context.SubjectExam
                    .Include(m => m.Subject) // Bao gồm thông tin của Subject
                    .Where(m => m.ExamId == examID)
                    .ToListAsync();
            if(subjectExam == null) {
                return NotFound();
            }
            return subjectExam;
        }
        // GET: api/SubjectExam/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectExam>> GetSubjectExams(Guid id)
        {
            var subjectExams = await _context.SubjectExam.FindAsync(id);

            if (subjectExams == null)
            {
                return NotFound();
            }

            return subjectExams;
        }
        // PUT: api/SubjectExam/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubjectExams(Guid id, SubjectExam subjectExams)
        {
            if (id != subjectExams.SubjectExamID)
            {
                return BadRequest();
            }

            _context.Entry(subjectExams).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubjectExamsExists(id))
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
        [HttpPut("admin-update")]
        public async Task<string> AdminUpdate(Guid subjectExamID, bool checkData, bool dataUpdate)
        {
            if (!SubjectExamsExists(subjectExamID))
            {
                return "Dữ liệu không hợp lệ";
            } else {
                var subjectExam = await _context.SubjectExam.FindAsync(subjectExamID);
                if (checkData) {
                    var IsEnterCandidatesAbsent = subjectExam.IsEnterCandidatesAbsent;
                    if (IsEnterCandidatesAbsent == true && dataUpdate == false) {
                        subjectExam.IsEnterCandidatesAbsent = false;
                        subjectExam.IsMatchingTestScore = false;
                    } else if (IsEnterCandidatesAbsent == false && dataUpdate == true) {
                        subjectExam.IsEnterCandidatesAbsent = true;
                    } else {
                        return "Dữ liệu không hợp lệ";
                    }
                    _context.Entry(subjectExam).State = EntityState.Modified;
                } else {
                    var IsMatchingTestScore = subjectExam.IsMatchingTestScore;
                    if (IsMatchingTestScore == true && dataUpdate == false) {
                        subjectExam.IsMatchingTestScore = false;
                    } else if (IsMatchingTestScore == false && dataUpdate == true) {
                        subjectExam.IsMatchingTestScore = true;
                        subjectExam.IsEnterCandidatesAbsent = true;
                    } else {
                        return "Dữ liệu không hợp lệ";
                    }
                    _context.Entry(subjectExam).State = EntityState.Modified;
                }
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                return "Dữ liệu không hợp lệ";
            }
            return "Done";
        }
        // POST: api/SubjectExam
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SubjectExam>> PostSubjectExams(SubjectExam subjectExams)
        {
            _context.SubjectExam.Add(subjectExams);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubjectExams", new { id = subjectExams.SubjectExamID }, subjectExams);
        }
        // DELETE: api/SubjectExam/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubjectExams(Guid id)
        {
            var subjectExams = await _context.SubjectExam.FindAsync(id);
            if (subjectExams == null)
            {
                return NotFound();
            }
            _context.SubjectExam.Remove(subjectExams);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("download-excel-file")]
        public async Task<IActionResult> DownloadExcel(int examID, bool isEnterCandidatesAbsent, bool isMatchingTestScore)
        {
            var subjectExams = await (from se in _context.SubjectExam
                                join s in _context.Subject on se.SubjectCode equals s.SubjectCode
                                where se.ExamId == examID && se.IsEnterCandidatesAbsent == isEnterCandidatesAbsent && se.IsMatchingTestScore == isMatchingTestScore
                                select new SubjectExamExportVM
                                {
                                    SubjectCode = s.SubjectCode,
                                    SubjectName = s.SubjectName,
                                    ExamBag = se.ExamBag,
                                    IsEnterCandidatesAbsent = se.IsEnterCandidatesAbsent?"Đã nhập":"Chưa nhập",
                                    IsMatchingTestScore = se.IsMatchingTestScore?"Đã nhập":"Chưa nhập"
                                }).ToListAsync();
            // Tạo một workbook Excel mới
            using (var package = new ExcelPackage())
            {
                var worksheet1 = package.Workbook.Worksheets.Add("sheet1");
                worksheet1.Cells["A1"].Value = "#";
                worksheet1.Cells["B1"].Value = "Mã môn";
                worksheet1.Cells["C1"].Value = "Tên môn";
                worksheet1.Cells["D1"].Value = "Túi bài thi";
                worksheet1.Cells["E1"].Value = "Nhập vắng thi";
                worksheet1.Cells["F1"].Value = "Nhập Điểm";
                worksheet1.Cells["A1:F1"].Style.Font.Bold = true;
                for (int i = 0; i < subjectExams.Count; i++)
                {
                    subjectExams[i].RowNumber = i + 1;
                }
                worksheet1.Cells["A2"].LoadFromCollection(subjectExams, false);
                var headerRange1 = worksheet1.Cells["A1:F1"];
                var dataRange1 = worksheet1.Cells["A1:F" + (subjectExams.Count + 1)];
                dataRange1.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                dataRange1.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                dataRange1.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                dataRange1.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                headerRange1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                dataRange1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet1.Cells[worksheet1.Dimension.Address].AutoFitColumns();
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
        private bool SubjectExamsExists(Guid id)
        {
            return _context.SubjectExam.Any(e => e.SubjectExamID == id);
        }
    }
}
