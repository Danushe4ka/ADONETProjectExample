using System;
using System.Collections.Generic;

namespace Lab2Places.Models;

public partial class PlaceInPack
{
    public int PipId { get; set; }

    public int? PlaceId { get; set; }

    public int? PackId { get; set; }

    public virtual Pack? Pack { get; set; }

    public virtual Place? Place { get; set; }
}
