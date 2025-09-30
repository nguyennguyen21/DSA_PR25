using System;
using System.Collections.Generic;

namespace DSA_PR25.Models;

public partial class Chapter
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
