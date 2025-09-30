using System;
using System.Collections.Generic;

namespace DSA_PR25.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string Fullname { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Role { get; set; }

    public int? Exp { get; set; }

    public int? BadgeId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Badge? Badge { get; set; }

    public virtual ICollection<Examresult> Examresults { get; set; } = new List<Examresult>();
}
