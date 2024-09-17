using System;
using System.Collections.Generic;

namespace Lab2Places.ViewModels;

public partial class AllPack
{
    public int PackId { get; set; }

    public string PackName { get; set; } = null!;

    public int PlaceId { get; set; }
}
