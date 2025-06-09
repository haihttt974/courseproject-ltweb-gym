using System;
using System.Collections.Generic;

namespace gym.Data;

public partial class Payment
{
    public int PaymentId { get; set; }

    public decimal? Amount { get; set; }

    public string? Method { get; set; }

    public virtual ICollection<MemberPayment> MemberPayments { get; set; } = new List<MemberPayment>();
}
