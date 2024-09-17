using System;
using System.Collections.Generic;

namespace Lab2Places.Models;

public partial class Place
{
    public int PlaceId { get; set; }

    public decimal? GeolocationA { get; set; }

    public decimal? GeolocationB { get; set; }

    public int? TypeId { get; set; }

    public string? PlaceDescription { get; set; }

    public short Rating { get; set; }

    public virtual ICollection<PlaceInPack> PlaceInPacks { get; set; } = new List<PlaceInPack>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual PlacesType? Type { get; set; }
}
