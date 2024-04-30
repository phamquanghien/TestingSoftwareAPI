using Microsoft.EntityFrameworkCore;
using TestingSoftwareAPI.Models;

namespace TestingSoftwareAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ActionHistory> ActionHistory { get; set; } = default!;
        public DbSet<Exam> Exam { get; set; } = default!;
        public DbSet<ExamResult> ExamResult { get; set; } = default!;
        public DbSet<RegistrationCode> RegistrationCode { get; set; } = default!;
        public DbSet<Student> Student { get; set; } = default!;
        public DbSet<StudentExam> StudentExam { get; set; } = default!;
        public DbSet<SubjectExam> SubjectExam { get; set; } = default!;
        public DbSet<Subject> Subject { get; set; } = default!;
    }
}
