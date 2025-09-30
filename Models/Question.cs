using System;
using System.Collections.Generic;

namespace DSA_PR25.Models;

public partial class Question
{
    public Guid Id { get; set; }

    public int? ChapterId { get; set; }

    /// <summary>
    /// r=Remember, u=Understand, ap=Apply, an=Analyze
    /// </summary>
    public string? BloomLevel { get; set; }

    public string? QuestionType { get; set; }

    public string? Img { get; set; }

    public string? Content { get; set; }

    public Guid? UpdatedBy { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Chapter? Chapter { get; set; }

    public virtual ICollection<Fillinblankexam> Fillinblankexams { get; set; } = new List<Fillinblankexam>();

    public virtual ICollection<Multiplechoiceexam> Multiplechoiceexams { get; set; } = new List<Multiplechoiceexam>();
}
