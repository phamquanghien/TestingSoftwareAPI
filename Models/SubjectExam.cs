using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestingSoftwareAPI.Models
{
    [Table("SubjectExams")]
    public class SubjectExams
    {
        [Key]
        public Guid SubjectExamID { get; set; }
        public int ExamId { get; set; }
        [Required]
        public string SubjectCode { get; set; }
        public bool IsEnterCandidatesAbsent { get; set; }
        public string UserEnterCandidatesAbsent { get; set; }
        public bool IsMatchingTestScore { get; set; }
        public string UserMatchingTestScore { get; set; }
        public DateTime CreateTime { get; set; }
        public string SubjectName { get; set; }
        public int ExamBag { get; set; }
        public bool? IsDownloadFileDiem { get; set; }
    }
}