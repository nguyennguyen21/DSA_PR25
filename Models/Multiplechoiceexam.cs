using System;
using System.Collections.Generic;

namespace DSA_PR25.Models;

public partial class Multiplechoiceexam
{
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }

    public string? OptionA { get; set; }

    public string? OptionB { get; set; }

    public string? OptionC { get; set; }

    public string? OptionD { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Multiplechoicecorrectanswer> Multiplechoicecorrectanswers { get; set; } = new List<Multiplechoicecorrectanswer>();

    public virtual Question Question { get; set; } = null!;
}
