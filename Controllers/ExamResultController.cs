using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using OfficeOpenXml;
using SQLitePCL;
using TestingSoftwareAPI.Data;
using TestingSoftwareAPI.Models;
using TestingSoftwareAPI.Models.Process;
using TestingSoftwareAPI.Models.ViewModel;

namespace TestingSoftwareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamResultController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private ExcelProcess _excelProcess = new ExcelProcess();
        private StudentProcess _studentProcess = new StudentProcess();
        private SubjectProcess _subjectProcess = new SubjectProcess();
        private SubjectExamProcess _subjectExamProcess = new SubjectExamProcess();

        public ExamResultController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ExamResult
        [HttpGet("get-by-examID-examBag-all")]
        public async Task<ActionResult<IEnumerable<ExamResult>>> GetExamResultAll(int examID, int examBag)
        {
            return await _context.ExamResult.Where(m => m.ExamId == examID && m.ExamBag == examBag).ToListAsync();
        }
        [HttpGet("get-by-examID-examBag")]
        public async Task<ActionResult<IEnumerable<ExamResult>>> GetExamResult(int examID, int examBag)
        {
            return await _context.ExamResult.Where(m => m.ExamId == examID && m.ExamBag == examBag && m.IsActive == true).ToListAsync();
        }
        [HttpGet("admin-get-by-student-code")]
        public async Task<ActionResult<IEnumerable<ExamResult>>> AdminGetByStudentCode (int examID, string subjectCode, string studentCode)
        {
            var examResult = await (from er in _context.ExamResult
                            join rc in _context.RegistrationCode on er.RegistrationCodeNumber equals rc.RegistrationCodeNumber
                            join se in _context.StudentExam on rc.StudentExamID equals se.StudentExamID
                            join std in _context.Student on se.StudentCode equals std.StudentCode
                            where se.ExamId == examID && se.StudentCode == studentCode && se.SubjectCode == subjectCode
                            select er
                            ).ToListAsync();
            return examResult;
        }

        // GET: api/ExamResult/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamResult>> GetExamResult(Guid id)
        {
            var examResult = await _context.ExamResult.FindAsync(id);

            if (examResult == null)
            {
                return NotFound();
            }

            return examResult;
        }
        [HttpGet("admin-download-by-exam-bag")]
        public async Task<IActionResult> AdminDownloadByExamBag(int examID, int examBag)
        {
            var examResult = await (from regCode in _context.RegistrationCode
                            join er in _context.ExamResult on regCode.RegistrationCodeNumber equals er.RegistrationCodeNumber
                            join stdEx in _context.StudentExam on regCode.StudentExamID equals stdEx.StudentExamID
                            join std in _context.Student on stdEx.StudentCode equals std.StudentCode
                            where er.ExamId == examID && er.ExamBag == examBag
                            select new ExamResultVM {
                                StudentCode = stdEx.StudentCode,
                                LastName = std.LastName,
                                FirstName = std.FirstName,
                                AverageScore = er.AverageScore
                            }).ToListAsync();
            using (var package = new ExcelPackage())
            {
                var worksheet1 = package.Workbook.Worksheets.Add("sheet1");
                worksheet1.Cells["A1"].Value = "#";
                worksheet1.Cells["B1"].Value = "Mã Sinh viên";
                worksheet1.Cells["C1"].Value = "Họ lót";
                worksheet1.Cells["D1"].Value = "Tên";
                worksheet1.Cells["E1"].Value = "Điểm";
                worksheet1.Cells["A1:E1"].Style.Font.Bold = true;
                for (int i = 0; i < examResult.Count; i++)
                {
                    examResult[i].RowNumber = i + 1;
                }
                worksheet1.Cells["A2"].LoadFromCollection(examResult, false);
                var headerRange1 = worksheet1.Cells["A1:E1"];
                var dataRange1 = worksheet1.Cells["A1:E" + (examResult.Count + 1)];
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
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExamResult.xlsx");
            }
        }

        // PUT: api/ExamResult/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExamResult(Guid id, ExamResult examResult)
        {
            if (id != examResult.ExamResultID)
            {
                return BadRequest();
            }
            var updateExamResult = await _context.ExamResult.FindAsync(id);
            try {
                var score = Convert.ToDouble(examResult.ReviewScore);
                if (score >= 0 && score <= 10) {
                    updateExamResult.ReviewScore = score.ToString();
                    updateExamResult.AverageScore = score.ToString();
                    updateExamResult.IsReview = true;
                }
                else {
                    return Ok("Dữ liệu không hợp lệ!");
                }
            } catch {
                return BadRequest();
            }
            
            _context.Entry(updateExamResult).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Cập nhật điểm thi thành công");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExamResultExists(id))
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

        // POST: api/ExamResult
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ExamResult>> PostExamResult(ExamResult examResult)
        {
            _context.ExamResult.Add(examResult);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExamResult", new { id = examResult.ExamResultID }, examResult);
        }

        // DELETE: api/ExamResult/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExamResult(Guid id)
        {
            var examResult = await _context.ExamResult.FindAsync(id);
            if (examResult == null)
            {
                return NotFound();
            }

            _context.ExamResult.Remove(examResult);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("scan-registration-code-number")]
        public async Task<string> LectureScanRegistrationCodeNumber([FromBody] List<LecturersScanCodeVM> lecturersScanCodes, int examId, int examBag)
        {
            var messageResult = await MatchingTestScore(lecturersScanCodes, examId, examBag);
            return messageResult;
        }

        [HttpPost("upload-file-result")]
        public async Task<IActionResult> Upload(IFormFile file, int examId, int examBag)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is required.");
            // Kiểm tra loại file nếu cần thiết
            if (Path.GetExtension(file.FileName).ToLower() != ".xlsx" && Path.GetExtension(file.FileName).ToLower() != ".xls")
            {
                return BadRequest("Only Excel files are allowed.");
            }
            try
            {
                string uploadsFolder = Path.Combine("Uploads","Excels");
                string uniqueFileName = "File" + Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                Directory.CreateDirectory(uploadsFolder);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    var fileLocation = new FileInfo(filePath).ToString();
                    var listVM = getListLecturersScanCodeVMFromExcel(fileLocation);
                    var result = await MatchingTestScore(listVM, examId, examBag);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        private async Task<string> MatchingTestScore(List<LecturersScanCodeVM> lecturersScanCodes, int examId, int examBag)
        {
            string messageResult = "";
            messageResult = await CheckDataFromListVM(lecturersScanCodes, examId);
            if(messageResult.Length == 0) {
                var listVT = await (from regisCode in _context.RegistrationCode
                                    join stdExam in _context.StudentExam on regisCode.StudentExamID equals stdExam.StudentExamID
                                    where stdExam.ExamId == examId && stdExam.ExamBag == examBag && stdExam.IsActive == false
                                    select regisCode.RegistrationCodeNumber).ToListAsync();
                var listDT = await (from regisCode in _context.RegistrationCode
                                    join stdExam in _context.StudentExam on regisCode.StudentExamID equals stdExam.StudentExamID
                                    where stdExam.ExamId == examId && stdExam.ExamBag == examBag && stdExam.IsActive == true
                                    select regisCode.RegistrationCodeNumber).ToListAsync();
                List<int> registrationCodeNumberList = new List<int>();
                for(int i  = 0; i < lecturersScanCodes.Count(); i++)
                {
                    registrationCodeNumberList.Add(lecturersScanCodes[i].RegistrationCodeNumber);
                }
                var listExcelVT = registrationCodeNumberList.Except(listDT);
                var listThieuBaiThi = listDT.Except(registrationCodeNumberList);
                if(listExcelVT.Count() == 0 && listThieuBaiThi.Count() == 0) {
                    var subjectExamCode = await _context.SubjectExam.Where(m => m.ExamId == examId && m.ExamBag == examBag).Select(m => m.SubjectExamID).FirstOrDefaultAsync();
                    var updateSubjectExam = await _context.SubjectExam.FindAsync(subjectExamCode);
                    SubjectExam.MatchingTestScore status = updateSubjectExam.MatchingTestScoreStatus;
                    if(status == SubjectExam.MatchingTestScore.EnteredMatchingTestScore || status == SubjectExam.MatchingTestScore.ReenterMatchingTestScore) {
                        //remove data exam result old
                        var listExamResultOld = await _context.ExamResult.Where(m => m.ExamId == examId && m.ExamBag == examBag).ToListAsync();
                        _context.RemoveRange(listExamResultOld);
                    }
                    var subjectCode = _context.SubjectExam.Where(m => m.ExamId == examId && m.ExamBag == examBag).FirstOrDefault().SubjectCode;
                    var importListVT = listVT.Select(registrationCodeNumber => new ExamResult
                                            {
                                                RegistrationCodeNumber = registrationCodeNumber,
                                                ExamResult1 = "V",
                                                ExamResult2 = "V",
                                                AverageScore = "V",
                                                ExamId = examId,
                                                ExamBag = examBag,
                                                IsActive = false,
                                                SubjectCode = subjectCode
                                            }).ToList();
                    await _context.ExamResult.AddRangeAsync(importListVT);
                    var examResultList = lecturersScanCodes.Select(m => new ExamResult {
                            RegistrationCodeNumber = m.RegistrationCodeNumber,
                            ExamResult1 = "" + m.ExamResult1,
                            ExamResult2 = "" + m.ExamResult2,
                            AverageScore = "" + m.AverageScore,
                            ExamId = examId,
                            ExamBag = examBag,
                            IsActive = true,
                            SubjectCode = subjectCode
                        }).ToList();
                    await _context.ExamResult.AddRangeAsync(examResultList);
                    if (updateSubjectExam == null)
                    {
                        messageResult = "Yêu cầu thực hiện không thành công, không thể cập nhật trạng thái túi bài thi. Vui lòng thử lại sau";
                    } else {
                        updateSubjectExam.IsMatchingTestScore = true;
                        updateSubjectExam.UserMatchingTestScore = "Username Login";
                        SubjectExam.MatchingTestScore status2 = updateSubjectExam.MatchingTestScoreStatus;
                        if(status2 == SubjectExam.MatchingTestScore.NotMatchingTestScore) {
                            updateSubjectExam.MatchingTestScoreStatus = SubjectExam.MatchingTestScore.EnteredMatchingTestScore;
                        } else if(status2 == SubjectExam.MatchingTestScore.EnteredMatchingTestScore){
                            updateSubjectExam.MatchingTestScoreStatus = SubjectExam.MatchingTestScore.ReenterMatchingTestScore;
                        }
                        _context.Entry(updateSubjectExam).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        messageResult = "Nhập điểm thành công";
                    }
                } else if(listExcelVT.Count() > 0) {
                    var checkListVT = listExcelVT.Except(listVT);
                    if(checkListVT.Count() == 0) {
                        messageResult = "Các sinh viên: ";
                        foreach(var item in listExcelVT) {
                            messageResult += item + ", ";
                        }
                        messageResult += " đã tích vắng thi nhưng vẫn có điểm, vui lòng liên hệ quản trị viên để được hỗ trợ!";
                    }
                    else {
                        messageResult = "Các mã phách: ";
                        foreach(var item in checkListVT) {
                            messageResult += item + ", ";
                        }
                        messageResult += " không đúng vui lòng kiểm tra lại";
                    }
                } else if(listThieuBaiThi.Count() > 0) {
                    messageResult = "Còn thiếu " + listThieuBaiThi.Count() + " bài thi hoặc danh sách vắng thi bị sai, vui lòng liên hệ với quản trị viên để được hỗ trợ!";
                }
            }
            return messageResult;
        }
        private async Task<string> CheckDataFromListVM(List<LecturersScanCodeVM> lecturersScanCodes, int examID)
        {
            string messageResult = "";
            if (lecturersScanCodes.Count() == 0) messageResult = "Dữ liệu chưa có vui lòng kiểm tra lại!";
            else {
                HashSet<int> seenRegistrationCodeNumbers = new HashSet<int>();
                var exam = await _context.Exam.FindAsync(examID);
                int startRegistrationCode = exam.StartRegistrationCode;
                for(int i = 0; i < lecturersScanCodes.Count(); i++)
                {
                    bool checkData = true;
                    try {
                        if(Convert.ToDouble(lecturersScanCodes[i].RegistrationCodeNumber.ToString()) < startRegistrationCode) checkData = false;
                        double examResult1 = Convert.ToDouble(lecturersScanCodes[i].ExamResult1.ToString());
                        if(examResult1 < 0.0 || examResult1 > 10.0) checkData = false;
                        double examResult2 = Convert.ToDouble(lecturersScanCodes[i].ExamResult2.ToString());
                        if(examResult2 < 0.0 || examResult2 > 10.0) checkData = false;
                        double averageScore = Convert.ToDouble(lecturersScanCodes[i].AverageScore.ToString());
                        if(averageScore < 0.0 || averageScore > 10.0) checkData = false;
                        if (seenRegistrationCodeNumbers.Contains(lecturersScanCodes[i].RegistrationCodeNumber))
                        {
                            break;
                        } else {
                            seenRegistrationCodeNumbers.Add(lecturersScanCodes[i].RegistrationCodeNumber);
                        }
                        if(checkData == false) messageResult += (i+1) +"; ";
                    }
                    catch {
                        messageResult += (i+1) +"; ";
                    }
                }
                if(messageResult.Length > 0) messageResult = "Bản ghi: " + messageResult + " không hợp lệ";
                if(lecturersScanCodes.Count() != seenRegistrationCodeNumbers.Count())
                {
                    messageResult = "Dữ liệu không hợp lệ (Số phách bị trùng)!";
                }
            }
            return messageResult;
        }
        private List<LecturersScanCodeVM> getListLecturersScanCodeVMFromExcel(string fileLocation)
        {
            var dataFromExcel =  _excelProcess.ExcelToDataTable(fileLocation);
            var examResultList = dataFromExcel.AsEnumerable()
                                .Select(row => new LecturersScanCodeVM
                                {
                                    RegistrationCodeNumber = Convert.ToInt32(row.Field<string>(0)),
                                    ExamResult1 = Convert.ToDouble(row.Field<string>(1)),
                                    ExamResult2 = Convert.ToDouble(row.Field<string>(2)),
                                    AverageScore = (Convert.ToDouble(row.Field<string>(1)) + Convert.ToDouble(row.Field<string>(2)))/2
                                })
                                .ToList();
            return examResultList;
        }
        private bool ExamResultExists(Guid id)
        {
            return _context.ExamResult.Any(e => e.ExamResultID == id);
        }
    }
}
