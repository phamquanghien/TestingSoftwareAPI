using System.ComponentModel.DataAnnotations;

namespace TestingSoftwareAPI.Models.ViewModel
{
    public class ExamResultVM
    {
        public int RowNumber { get; set; }
        [Required]
        public string? StudentCode { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? AverageScore { get; set; }
    }
}