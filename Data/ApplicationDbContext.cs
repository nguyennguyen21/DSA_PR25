using DSA_PR25.Models;
using Microsoft.EntityFrameworkCore;

namespace  DSA_PR25.Data
{
    public class ApplicationDBcontext : DbContext
    {
        public ApplicationDBcontext(DbContextOptions<ApplicationDBcontext> options)
        : base(options)
        {
        }
        // Users
        public DbSet<User> Users { get; set; }

        // Chapters
        public DbSet<Chapter> Chapters { get; set; }

        // Questions
        public DbSet<Question> Questions { get; set; }

        // Fill-in-the-blank exams
        public DbSet<Fillinblankexam> Fillinblankexams { get; set; }
        public DbSet<Bloomscoring> Bloomscoring { get; set; }
        public DbSet<Examresult>    Examresults{ get; set; }
        public DbSet<Badge> Badges { get; set; }
       // MQC
        public DbSet<Multiplechoiceexam> Multiplechoiceexams { get; set; }
        public DbSet<Multiplechoicecorrectanswer> Multiplechoicecorrectanswers { get; set; }

    }
}