﻿using Parsers.Models;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TgBotForSearchWork.Models;
using TgBotForSearchWork.Services;
using TgBotForSearchWork.Utilities;

namespace TgBotForSearchWorkApi.Services;

public class VacancyBackgroundService : BackgroundService
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly VacancyService _vacancyService;
    private readonly UserRepository _userRepository;
    private readonly TimeSpan _timeOut;

    public VacancyBackgroundService(ITelegramBotClient telegramBotClient, VacancyService vacancyService, 
                                    UserRepository userRepository, IConfiguration configuration)
    {
        _vacancyService = vacancyService;
        _telegramBotClient = telegramBotClient;
        _userRepository = userRepository;
        _timeOut = TimeSpan.FromSeconds(configuration.GetValue<int>("TimeOutBetweenSendVacancies"));
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            foreach (var user in _userRepository.GetAll(cancellationToken))
                await SendVacancyAsync(user, cancellationToken);
            await Task.Delay(_timeOut, cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Info(ex.Message);
        }
    }
    private async Task SendVacancyAsync(User user, CancellationToken cancellationToken)
    {
        List<Vacancy> relevantVacancies = await _vacancyService.GetRelevantVacanciesAsync(user, cancellationToken);
        _userRepository.Update(user, cancellationToken);
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