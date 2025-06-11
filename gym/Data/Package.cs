using System;
using System.Collections.Generic;

namespace gym.Data;

public partial class Package
{
    public int PackageId { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public decimal? Price { get; set; }

    public int? DurationInDays { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<MemberPackage> MemberPakages { get; set; } = new List<MemberPackage>();
    public double Duration { get; internal set; }
}
