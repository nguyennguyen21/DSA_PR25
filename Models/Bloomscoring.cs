using System;
using System.Collections.Generic;

namespace DSA_PR25.Models;

public partial class Bloomscoring
{
    public int Id { get; set; }

    /// <summary>
    /// r=Remember, u=Understand, ap=Apply, an=Analyze
    /// </summary>
    public string BloomLevel { get; set; } = null!;

    public float ScoreMultiplier { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
