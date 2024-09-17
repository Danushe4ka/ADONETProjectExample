using System;
using System.Collections.Generic;

namespace Lab2Places.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserLogin { get; set; } = null!;

    public string UserPassword { get; set; } = null!;

    public string? Email { get; set; }

    public virtual ICollection<Pack> Packs { get; set; } = new List<Pack>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
