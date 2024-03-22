using System.ComponentModel.DataAnnotations;

namespace TestingSoftwareAPI.Models
{
    public class Exam
    {
        [Key]
        public int ExamId { get; set; }
        [Display(Name = "Mã kỳ thi")]
        [Required]
        public string ExamCode { get; set; }
        [Display(Name = "Tên kỳ thi")]
        [Required]
        public string ExamName { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Ngày tạo")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        [Display(Name = "Người tạo")]
        [Required]
        public string CreatePerson { get; set; }
        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        public bool? IsDelete { get; set; } = false;

        public bool? Status { get; set; } = false;
        [Display(Name = "Phách bắt đầu")]
        public int StartRegistrationCode { get; set; } = 10000;
    }
}