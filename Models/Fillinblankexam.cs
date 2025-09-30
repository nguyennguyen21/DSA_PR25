using System;
using System.Collections.Generic;

namespace DSA_PR25.Models;

public partial class Fillinblankexam
{
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }

    public double? Tolerance { get; set; }

    public string? Synonyms { get; set; }

    public string CorrectAnswer { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Question Question { get; set; } = null!;
}
