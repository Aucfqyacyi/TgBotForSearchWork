using TgBotForSearchWorkApi.Utilities;

namespace TgBotForSearchWorkApi.Services;

public class FilterBackgroundService : BackgroundService
{
    private readonly FilterService _filterService;

    public FilterBackgroundService(FilterService filterService)
    {
        _filterService = filterService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _filterService.CollectFiltersAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            Log.Info($"Filters were not parsed, error {ex.Message}");
            throw;
        }

    }
}
