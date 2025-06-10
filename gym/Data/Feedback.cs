using System;

namespace gym.Data
{
    public partial class Feedback
    {
        public int FeedbackId { get; set; }
        public int UserId { get; set; }
        public string? Content { get; set; }
        public DateTime ThoiGian { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
