using gym.Data;
using Microsoft.EntityFrameworkCore;

public class ExpiredPackageChecker : IHostedService, IDisposable
{
    private Timer? _timer;
    private readonly IServiceScopeFactory _scopeFactory;

    public ExpiredPackageChecker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Chạy mỗi 24h (86400 giây)
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(24));
        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GymContext>();

        var today = DateTime.Today;
        var expiredPackages = await context.MemberPakages
            .Where(p => p.IsActive == true && p.EndDate < today)
            .ToListAsync();

        foreach (var pkg in expiredPackages)
        {
            pkg.IsActive = false;
        }

        await context.SaveChangesAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
