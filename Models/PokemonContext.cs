using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace DSA_PR25.Models;

public partial class PokemonContext : DbContext
{
    public PokemonContext()
    {
    }

    public PokemonContext(DbContextOptions<PokemonContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Badge> Badges { get; set; }

    public virtual DbSet<Bloomscoring> Bloomscorings { get; set; }

    public virtual DbSet<Chapter> Chapters { get; set; }

    public virtual DbSet<Examresult> Examresults { get; set; }

    public virtual DbSet<Fillinblankexam> Fillinblankexams { get; set; }

    public virtual DbSet<Multiplechoicecorrectanswer> Multiplechoicecorrectanswers { get; set; }

    public virtual DbSet<Multiplechoiceexam> Multiplechoiceexams { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=127.0.0.1;user=root;password=1234;database=pokemon", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.43-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Badge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("badges");

            entity.Property(e => e.Image).HasMaxLength(1000);
            entity.Property(e => e.Name).HasMaxLength(155);
        });

        modelBuilder.Entity<Bloomscoring>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("bloomscoring");

            entity.HasIndex(e => e.BloomLevel, "BloomLevel").IsUnique();

            entity.Property(e => e.BloomLevel)
                .HasComment("r=Remember, u=Understand, ap=Apply, an=Analyze")
                .HasColumnType("enum('r','u','ap','an')");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ScoreMultiplier).HasDefaultValueSql("'1'");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("chapters");

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Examresult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("examresults");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.BloomScore).HasDefaultValueSql("'0'");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.ExamDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Examresults)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("examresults_ibfk_1");
        });

        modelBuilder.Entity<Fillinblankexam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("fillinblankexams");

            entity.HasIndex(e => e.QuestionId, "QuestionId");

            entity.Property(e => e.CorrectAnswer).HasColumnType("text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Synonyms).HasColumnType("text");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Question).WithMany(p => p.Fillinblankexams)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("fillinblankexams_ibfk_1");
        });

        modelBuilder.Entity<Multiplechoicecorrectanswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("multiplechoicecorrectanswers");

            entity.HasIndex(e => e.ExamId, "ExamId");

            entity.Property(e => e.CorrectOption).HasColumnType("enum('A','B','C','D')");

            entity.HasOne(d => d.Exam).WithMany(p => p.Multiplechoicecorrectanswers)
                .HasForeignKey(d => d.ExamId)
                .HasConstraintName("multiplechoicecorrectanswers_ibfk_1");
        });

        modelBuilder.Entity<Multiplechoiceexam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("multiplechoiceexams");

            entity.HasIndex(e => e.QuestionId, "QuestionId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.OptionA).HasColumnType("text");
            entity.Property(e => e.OptionB).HasColumnType("text");
            entity.Property(e => e.OptionC).HasColumnType("text");
            entity.Property(e => e.OptionD).HasColumnType("text");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Question).WithMany(p => p.Multiplechoiceexams)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("multiplechoiceexams_ibfk_1");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("questions");

            entity.HasIndex(e => e.ChapterId, "ChapterId");

            entity.Property(e => e.BloomLevel)
                .HasComment("r=Remember, u=Understand, ap=Apply, an=Analyze")
                .HasColumnType("enum('r','u','ap','an')");
            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Img).HasMaxLength(1000);
            entity.Property(e => e.QuestionType).HasColumnType("enum('mcq','fn','fns','fss','fps','fe','mp','pe')");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Chapter).WithMany(p => p.Questions)
                .HasForeignKey(d => d.ChapterId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("questions_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.BadgeId, "BadgeId");

            entity.HasIndex(e => e.Username, "Username").IsUnique();

            entity.Property(e => e.BadgeId).HasDefaultValueSql("'1'");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Exp).HasDefaultValueSql("'0'");
            entity.Property(e => e.Fullname).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'student'")
                .HasColumnType("enum('admin','student')");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(36);

            entity.HasOne(d => d.Badge).WithMany(p => p.Users)
                .HasForeignKey(d => d.BadgeId)
                .HasConstraintName("users_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
