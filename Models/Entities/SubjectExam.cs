using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestingSoftwareAPI.Models
{
    [Table("SubjectExams")]
    public class SubjectExam
    {
        [Key]
        public Guid SubjectExamID { get; set; }
        public int ExamId { get; set; }
        public bool IsEnterCandidatesAbsent { get; set; } = false;
        public string? UserEnterCandidatesAbsent { get; set; }
        public bool IsMatchingTestScore { get; set; } = false;
        public string? UserMatchingTestScore { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public int ExamBag { get; set; }
        public bool? IsDownloadFileDiem { get; set; } = false;
        public CandidatesAbsentStatus EnterCandidatesAbsentStatus { get; set; } = 0;
        // Kiểu Enum mới
        public enum CandidatesAbsentStatus
        {
            [Display(Name = "Chưa nhập danh sách vắng thi")]
            NotEnteredAbsentList = 0,

            [Display(Name = "Đã nhập danh sách vắng thi")]
            EnteredAbsentList = 1,

            [Display(Name = "Nhập lại danh sách vắng thi")]
            ReenterAbsentList = 2
        }
        public MatchingTestScore MatchingTestScoreStatus { get; set; } = 0;
        // Kiểu Enum mới
        public enum MatchingTestScore
        {
            [Display(Name = "Chưa ghép điểm thi")]
            NotMatchingTestScore = 0,

            [Display(Name = "Đã ghép điểm thi")]
            EnteredMatchingTestScore = 1,

            [Display(Name = "Ghép lại điểm thi")]
            ReenterMatchingTestScore = 2
        }
        [Required]
        public string? SubjectCode { get; set; }
        [ForeignKey("SubjectCode")]
        public Subject? Subject { get; set; }
    }
}