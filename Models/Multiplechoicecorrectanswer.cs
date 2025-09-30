using System;
using System.Collections.Generic;

namespace DSA_PR25.Models;

public partial class Multiplechoicecorrectanswer
{
    public Guid Id { get; set; }

    public Guid ExamId { get; set; }

    public string CorrectOption { get; set; } = null!;

    public virtual Multiplechoiceexam Exam { get; set; } = null!;
}
