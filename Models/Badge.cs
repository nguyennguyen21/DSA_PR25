using System;
using System.Collections.Generic;

namespace DSA_PR25.Models;

public partial class Badge
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Image { get; set; }

    public int BadgeLevel { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
