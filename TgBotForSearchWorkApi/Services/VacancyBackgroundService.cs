using Amazon.Runtime.Internal.Transform;
using AngleSharp.Html;
using MongoDB.Bson.Serialization.Serializers;
using Parsers.Models;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Repositories;
using TgBotForSearchWorkApi.Services;
using TgBotForSearchWorkApi.Utilities;

namespace TgBotForSearchWorkApi.Services;

public class VacancyBackgroundService : BackgroundService
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly VacancyService _vacancyService;
    private readonly TimeSpan _timeout;
    private readonly int _urisLimit;
    private int _skip = 0;
    private readonly UriToVacanciesRepository _uriToVacanciesRepository;

    public VacancyBackgroundService(ITelegramBotClient telegramBotClient, VacancyService vacancyService,
                                    UriToVacanciesRepository uriToVacanciesRepository, IConfiguration configuration)
    {
        _vacancyService = vacancyService;
        _telegramBotClient = telegramBotClient;
        _timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("TimeoutBetweenSendVacancies"));
        _urisLimit = configuration.GetValue<int>("UrisLimit");
        _uriToVacanciesRepository = uriToVacanciesRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested is false)
        {
            try
            {
                List<UriToVacancies> urisToVacancies = _uriToVacanciesRepository.GetAllActivated(_skip, _urisLimit, cancellationToken);
                var chatIdsToUris = GetChatIdsToUrisToVacancies(urisToVacancies);
                foreach (var chatIdToUris in chatIdsToUris)
                {
                    await SendVacancyAsync(chatIdToUris.Key, chatIdToUris.Value, cancellationToken);
                }              
                if (urisToVacancies.Count > 0)
                {
                    _skip += _urisLimit;
                    _uriToVacanciesRepository.UpdateManyLastVacancyIds(urisToVacancies, cancellationToken);
                }
                else
                {
                    _skip = 0;
                    await Task.Delay(_timeout, cancellationToken);                    
                }
            }
            catch (Exception ex)
            {
                Log.Info(ex.Message);
            }
        }       
    }

    private Dictionary<long, List<UriToVacancies>> GetChatIdsToUrisToVacancies(IEnumerable<UriToVacancies> urisToVacancies)
    {
        return urisToVacancies.Aggregate(new Dictionary<long, List<UriToVacancies>>(), (chatIdsToUris, uri) =>
        {
            List<UriToVacancies>? uris = chatIdsToUris.GetValueOrDefault(uri.ChatId);
            if (uris is null)
            {
                uris = new();
                chatIdsToUris.Add(uri.ChatId, uris);
            }               
            uris.Add(uri);            
            return chatIdsToUris;
        });
    }

    private async ValueTask SendVacancyAsync(long chatId, IEnumerable<UriToVacancies> urisToVacancies, CancellationToken cancellationToken)
    {
        List<Vacancy> relevantVacancies = await _vacancyService.GetRelevantVacanciesAsync(urisToVacancies, cancellationToken);
        await SendVacancyAsync(chatId, relevantVacancies, cancellationToken);
        Log.Info($"All vacancies for the chat({chatId}) were sent successfully.");
    }

    private async ValueTask SendVacancyAsync(long chatId, IReadOnlyList<Vacancy> vacancies, CancellationToken cancellationToken)
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

    private async ValueTask SendVacancyAsync(long chatId, Vacancy vacancy, CancellationToken cancellationToken)
    {
        await _telegramBotClient.SendTextMessageAsync(chatId,
                                                      vacancy.Present(),
                                                      ParseMode.Markdown,
                                                      disableWebPagePreview: true,
                                                      cancellationToken: cancellationToken);
    }
}
