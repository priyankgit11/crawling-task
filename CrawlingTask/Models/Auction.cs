using System;
using System.Collections.Generic;

namespace CrawlingTask.Models;

public partial class Auction
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? Link { get; set; }

    public int? LotCount { get; set; }

    public int? StartDate { get; set; }

    public int? StartMonth { get; set; }

    public int? StartYear { get; set; }

    public TimeOnly? StartTime { get; set; }

    public int? EndDate { get; set; }

    public int? EndMonth { get; set; }

    public int? EndYear { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string? Location { get; set; }
}
