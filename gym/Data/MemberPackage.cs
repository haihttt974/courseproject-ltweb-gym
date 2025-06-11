using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace gym.Data;

public partial class MemberPackage
{
    public int MemberId { get; set; }

    public int PackageId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? IsPaid { get; set; }

    public bool? IsActive { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual Package Package { get; set; } = null!;
    [NotMapped]
    public DateTime EndTime { get; set; }
}
