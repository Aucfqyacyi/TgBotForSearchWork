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
    private readonly ITelegramBotClient _telegramBotClient;  
    private readonly UserService _userService;

    public VacancySender(ITelegramBotClient telegramBotClient, UserService userService)
    {
        _userService = userService;
        _telegramBotClient = telegramBotClient;
    }

    public async Task SendVacancyAsync(TimeSpan timeOut, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var user in _userService.GetAllUsers(cancellationToken))
                await SendVacancyAsync(user, cancellationToken);
            await Task.Delay(timeOut, cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Info(ex.Message);
        }
    }

    private async Task SendVacancyAsync(User user, CancellationToken cancellationToken)
    {
        List<Vacancy> relevantVacancies = await _vacancyService.GetRelevantVacancies(user, cancellationToken);
        _userService.UpdateUser(user, cancellationToken);
        await SendVacancyAsync(user.ChatId, relevantVacancies, cancellationToken);
        Log.Info($"All vacancies for the user({user.ChatId}) were sent successfully.");
    }

    private async Task SendVacancyAsync(long chatId, IReadOnlyList<Vacancy> vacancies, CancellationToken cancellationToken)
    {
        for (int i = 0; i < vacancies.Count; i++)
        {
            try
            {
                await SendVacancyAsync(chatId, vacancies[i], cancellationToken);
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

    private Task SendVacancyAsync(long chatId, Vacancy vacancy, CancellationToken cancellationToken)
    {
        return _telegramBotClient.SendTextMessageAsync(chatId,
                                                      vacancy.Present(),
                                                      ParseMode.Markdown,
                                                      disableWebPagePreview: true,
                                                      cancellationToken: cancellationToken);
    }
}
