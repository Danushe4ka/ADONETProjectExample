using System;
using System.Collections.Generic;

namespace Lab2Places.ViewModels;

public partial class UserReview
{
    public int UserId { get; set; }

    public string UserLogin { get; set; } = null!;

    public string? PlaceDescription { get; set; }

    public byte Grade { get; set; }

    public string? ReviewText { get; set; }
}
