using System;
using System.Collections.Generic;

namespace DSA_PR25.Models;

public partial class Examresult
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public float TotalScore { get; set; }

    public int TotalQuestions { get; set; }

    public int CorrectAnswers { get; set; }

    public float? BloomScore { get; set; }

    public DateTime? ExamDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
