namespace TestingSoftwareAPI.Models.ViewModel
{
    public class ExamResultVM
    {
        public int RowNumber { get; set; }
        public string StudentCode { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string AverageScore { get; set; }
    }
}