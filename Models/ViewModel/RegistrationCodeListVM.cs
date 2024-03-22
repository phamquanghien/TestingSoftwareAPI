namespace TestingSoftwareAPI.Models.ViewModel
{
    public class RegistrationCodeListVM
    {
        public int RegistrationCodeNumber { get; set; }
        public Guid StudentExamID { get; set; }
        public int ExamId { get; set; }
        public string? StudentCode { get; set; }
        public string? LastName { get; set; }

        public string? FirstName { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
    }
}