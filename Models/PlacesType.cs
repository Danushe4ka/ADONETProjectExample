using System;
using System.Collections.Generic;
using Lab2Places.Models;

namespace Lab2Places;

public partial class PlacesType
{
    public int TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Place> Places { get; set; } = new List<Place>();
}
