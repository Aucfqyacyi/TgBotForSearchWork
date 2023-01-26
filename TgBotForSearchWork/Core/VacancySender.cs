using Parsers.Models;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using TgBotForSearchWork.Utilities;
using TgBotForSearchWork.Models;
using TgBotForSearchWork.Services;

namespace TgBotForSearchWork.Core;

internal class VacancySender
{
    private readonly VacancyService _vacancyService = new();
    private readonly UserService _userService;

    public VacancySender(UserService userService)
    {
        _userService = userService;
    }

    public async Task SendVacancyAsync(ITelegramBotClient telegramBotClient, TimeSpan timeOut, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var user in _userService.GetAllUsers(cancellationToken))
                await SendVacancyAsync(telegramBotClient, user, cancellationToken);
            await Task.Delay(timeOut, cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Info(ex.Message);
        }
    }

    private async Task SendVacancyAsync(ITelegramBotClient telegramBotClient, User user, CancellationToken cancellationToken)
    {
        List<Vacancy> relevantVacancies = await _vacancyService.GetRelevantVacancies(user, cancellationToken);
        _userService.UpdateUser(user, cancellationToken);
        await SendVacancyAsync(new(user.ChatId, telegramBotClient, cancellationToken), relevantVacancies);
        Log.Info($"All vacancies for the user({user.ChatId}) were sent successfully.");
    }

    private async Task SendVacancyAsync(TelegramEntity telegramEntity, IReadOnlyList<Vacancy> vacancies)
    {
        for (int i = 0; i < vacancies.Count; i++)
        {
            try
            {
                await SendVacancyAsync(telegramEntity, vacancies[i]);
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Log.Info($"Exception was thrown at vacancy({vacancies[i].Url})#{i},error - {ex.Message}");
            }
        }
    }

    private Task SendVacancyAsync(TelegramEntity telegramEntity, Vacancy vacancy)
    {
        return telegramEntity.TelegramBotClient.SendTextMessageAsync(telegramEntity.ChatId,
                                                      vacancy.Present(),
                                                      ParseMode.Markdown,
                                                      disableWebPagePreview: true,
                                                      cancellationToken: telegramEntity.CancellationToken);
    }
}
