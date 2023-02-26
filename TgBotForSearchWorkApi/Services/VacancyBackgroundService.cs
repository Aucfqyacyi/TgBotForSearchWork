using Parsers.Models;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Repositories;
using TgBotForSearchWorkApi.Utilities;

namespace TgBotForSearchWorkApi.Services;

public class VacancyBackgroundService : BackgroundService
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly VacancyService _vacancyService;
    private readonly UriToVacanciesRepository _uriToVacanciesRepository;
    private readonly UserRepository _userRepository;
    private readonly TimeSpan _timeout;
    private readonly int _urisLimit;
    private int _skip = 0;

    public VacancyBackgroundService(ITelegramBotClient telegramBotClient, VacancyService vacancyService,
                                    UriToVacanciesRepository uriToVacanciesRepository, IConfiguration configuration, 
                                    UserRepository userRepository)
    {
        _vacancyService = vacancyService;
        _telegramBotClient = telegramBotClient;
        _timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("TimeoutBetweenSendVacancies"));
        _urisLimit = configuration.GetValue<int>("UserLimit");
        _uriToVacanciesRepository = uriToVacanciesRepository;
        _userRepository = userRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested is false)
        {
            List<User> users = _userRepository.GetAllActivated(_skip, _urisLimit, cancellationToken);
            try
            {                 
                await SendVacanciesAsync(users, cancellationToken);                
            }
            catch (Exception ex)
            {
                Log.Info(ex.Message);
            }
            if (users.Count > 0)
            {
                _skip += _urisLimit;
            }
            else
            {
                _skip = 0;
                await Task.Delay(_timeout, cancellationToken);
            }
        }       
    }

    private async ValueTask SendVacanciesAsync(List<User> users, CancellationToken cancellationToken)
    {
        foreach (var user in users)
        {
            List<UriToVacancies> urisToVacancies = await SendVacanciesAsync(user, cancellationToken);
            if(urisToVacancies.Any())
                _uriToVacanciesRepository.UpdateManyLastVacancyIds(urisToVacancies, cancellationToken);
        }
    }

    private async ValueTask<List<UriToVacancies>> SendVacanciesAsync(User user, CancellationToken cancellationToken)
    {
        List<UriToVacancies> urisToVacancies = _uriToVacanciesRepository.GetAllActivated(user.ChatId, cancellationToken);
        if (urisToVacancies.Any())
            await SendVacanciesAsync(user, urisToVacancies, cancellationToken);
        return urisToVacancies;
    }

    private async ValueTask SendVacanciesAsync(User user, IEnumerable<UriToVacancies> urisToVacancies, CancellationToken cancellationToken)
    {
        List<Vacancy> relevantVacancies = await _vacancyService.GetRelevantVacanciesAsync(urisToVacancies, user.DescriptionLength, cancellationToken);
        await SendVacanciesAsync(user, relevantVacancies, cancellationToken);
        Log.Info($"All vacancies for the chat({user.ChatId}) were sent successfully.");
    }

    private async ValueTask SendVacanciesAsync(User user, IReadOnlyList<Vacancy> vacancies, CancellationToken cancellationToken)
    {
        for (int i = 0; i < vacancies.Count; i++)
        {
            try
            {
                await SendVacancyAsync(user.ChatId, vacancies[i], cancellationToken);
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

    private async ValueTask SendVacancyAsync(long chatId, Vacancy vacancy, CancellationToken cancellationToken)
    {
        await _telegramBotClient.SendTextMessageAsync(chatId,
                                                      vacancy.Present(),
                                                      ParseMode.Markdown,
                                                      disableWebPagePreview: true,
                                                      cancellationToken: cancellationToken);
    }
}
