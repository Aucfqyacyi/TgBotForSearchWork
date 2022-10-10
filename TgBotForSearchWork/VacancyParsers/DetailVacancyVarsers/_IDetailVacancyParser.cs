using TgBotForSearchWork.VacancyParsers.Models;

namespace TgBotForSearchWork.VacancyParsers.DetailVacancyVarsers;

public interface IDetailVacancyParser
{
    public Task<string> ParseAsync(Stream stream, CancellationToken cancellationToken = default);
}
