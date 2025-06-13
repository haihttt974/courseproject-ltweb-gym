using System;
using System.Collections.Generic;
using gym.Data;
using gym.Models;

public class AdminDashboardViewModel
{
    public int TotalMembers { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public int ExpiringSoonCount { get; set; }
    public List<Notification> LatestNotifications { get; set; }
}
