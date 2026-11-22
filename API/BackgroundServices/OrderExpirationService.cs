using Application.Interfaces.Services;

namespace API.BackgroundServices;

public class OrderExpirationService(IServiceScopeFactory scopeFactory, ILogger<OrderExpirationService> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
                await ExpireAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task ExpireAsync(CancellationToken ct)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var orders = scope.ServiceProvider.GetRequiredService<IOrderService>();

            var count = await orders.ExpirePendingOrdersAsync(ct);
            if (count > 0)
                logger.LogInformation("Истекло заказов: {Count}", count);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при истечении заказов");
        }
    }
}