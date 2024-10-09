namespace TestingSoftwareAPI.Models.ViewModel
{
    public class SubjectExamExportVM
    {
        public int RowNumber { get; set; }
        public string? SubjectCode { get; set; }
        public string? SubjectName { get; set; }
        public int ExamBag { get; set; }
        public string? IsEnterCandidatesAbsent { get; set; }
        public string? IsMatchingTestScore { get; set; }
    }
}