namespace gym.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalMembers { get; set; }
        public int TotalActivePackages { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<PackageStat> PackageStats { get; set; } = new();
    }

    public class PackageStat
    {
        public string PackageName { get; set; } = string.Empty;
        public int MemberCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
