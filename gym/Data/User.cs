using System;
using System.Collections.Generic;

namespace gym.Data;

public partial class User
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public int? ReferenceId { get; set; }

    public string? Status { get; set; }

    public bool? IsAtive { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}
