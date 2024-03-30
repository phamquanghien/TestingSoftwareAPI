using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestingSoftwareAPI.Models
{
    [Table("ExamResults")]
    public class ExamResult
    {
        [Key]
        public Guid ExamResultID { get; set; }
        public int ExamCodeNumber { get; set; }
        [MaxLength(5)]
        public string ExamResult1 { get; set; }
        [MaxLength(5)]
        public string ExamResult2 { get; set; }
        [MaxLength(5)]
        public string AverageScore { get; set; }
        [MaxLength(5)]
        public string ReviewScore { get; set; }
        public int ExamId { get; set; }
        [Required]
        public string SubjectCode { get; set; }
        public bool IsActive { get; set; }
        public int ExamBag { get; set; }
        public bool? IsReview { get; set; }
    }
}