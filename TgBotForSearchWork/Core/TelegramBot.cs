using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TgBotForSearchWork.Constants;
using TgBotForSearchWork.Managers.UserManagers;
using TgBotForSearchWork.Models;
using TgBotForSearchWork.Others;
using TgBotForSearchWork.VacancyParsers;
using User = TgBotForSearchWork.Models.User;

namespace TgBotForSearchWork.Core;

public class TelegramBot
{
	private readonly TelegramBotClient _telegramBotClient;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ReceiverOptions _receiverOptions;
    private readonly UserManager _userManager;
    private readonly TimeSpan _timeOut;

    public TelegramBot(string token, TimeSpan timeOut, UserManager userManager, ReceiverOptions? receiverOptions = null)
    {
        _telegramBotClient = new(token, GHttpClient.Client);
        receiverOptions ??= new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() };
        _receiverOptions = receiverOptions;
        _userManager = userManager;
        _timeOut = timeOut;
    }

    public async Task StartAsync()
    {
        _telegramBotClient.StartReceiving(
                    updateHandler: HandleUpdateAsync,
                    pollingErrorHandler: HandlePollingErrorAsync,
                    receiverOptions: _receiverOptions,
                    cancellationToken: _cancellationTokenSource.Token);

        while (_cancellationTokenSource.Token.IsCancellationRequested is false)
        {
            try
            {
                foreach (var user in _userManager.Users)
                    await SendVacancyAsync(user, _cancellationTokenSource.Token);
                await Task.Delay(_timeOut, _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                Log.Info($"Cancellation is requested in main token = {_cancellationTokenSource.Token.IsCancellationRequested}.");
            }
            catch (Exception ex)
            {
                Log.Info(ex.Message);
            }
        }
             
    }

    private async Task SendVacancyAsync(User user, CancellationToken cancellationToken = default)
    {
        foreach (var urlToVacancy in user.UrlsToVacancies)
        {
            if (urlToVacancy.Key.IsOff)
            {                
                IReadOnlyList<Vacancy> vacancies = await GetRelevantVacancies( urlToVacancy.Key, urlToVacancy.Value, cancellationToken);
                if (vacancies.Count != 0)
                {
                    await SendVacancyAsync(user.ChatId, vacancies, cancellationToken);
                    user.UrlsToVacancies[urlToVacancy.Key] = vacancies.FirstOrDefault();
                }
                Log.Info($"{urlToVacancy.Key.Host} has number of vacancies {vacancies.Count}");
            }
        }
    }

    private async Task SendVacancyAsync(long chatId, IEnumerable<Vacancy> vacancies, CancellationToken cancellationToken = default)
    {
        foreach (Vacancy vacancy in vacancies)
        {
            await SendVacancyAsync(chatId, vacancy, cancellationToken);
        }
    }

    private Task SendVacancyAsync(long chatId, Vacancy vacancy, CancellationToken cancellationToken = default)
    {
        return _telegramBotClient.SendTextMessageAsync(chatId, 
                                                      vacancy.Present(), 
                                                      ParseMode.Markdown,
                                                      disableWebPagePreview: true,
                                                      cancellationToken: cancellationToken);
    }

    private async Task<IReadOnlyList<Vacancy>> GetRelevantVacancies(Uri uri, Vacancy? lastVacancy, CancellationToken cancellationToken = default)
    {
        IVacancyParser vacancyParser = VacancyParserFactory.CreateVacancyParser(uri);
        List<Vacancy> vacancies = await vacancyParser.ParseAsync(uri, cancellationToken);
        if (lastVacancy is not null)
        {
            if (lastVacancy == vacancies.FirstOrDefault())
            {
                return new List<Vacancy>();
            }
            int index = vacancies.FindIndex(vacancy => vacancy.Url == lastVacancy.Url);
            if (index != -1)
            {
                vacancies.RemoveRange(index, vacancies.Count - index);
            }
        }
        return vacancies;
    }

    public void Stop()
    {
        if (_cancellationTokenSource.IsCancellationRequested is false)
        {     
            _cancellationTokenSource.Cancel();
        }
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default)
    {      
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    await OnMessage(botClient, update, cancellationToken);
                    break;
            }       
        }
        catch (Exception ex)
        {
            Log.Info(ex.Message);
        }
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken = default)
    {
        string errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Log.Info(errorMessage);
        return Task.CompletedTask;
    }

    private async Task OnMessage(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default)
    {
        string messageText = update.Message!.Text!;
        long chatId = update.Message!.Chat.Id;
        if (messageText.Contains(Command.ShowAllUrls))
            await OnShowAllUrls(chatId, cancellationToken);
        if (messageText.Contains(Command.Start))
            _userManager.AddUser(chatId);
        if (messageText.Contains(Command.Stop))
            _userManager.RemoveUser(chatId);
        if (messageText.Contains(Command.Test))
        {
            await botClient.SendTextMessageAsync(chatId, "Hello, I am friendly neighborhood bot <3.", cancellationToken: cancellationToken);
            Log.Info($"Test command was called.");
        }          
    }

    private async Task OnShowAllUrls(long chatId, CancellationToken cancellationToken = default)
    {
        User? user = _userManager.Users.FirstOrDefault(user => user.ChatId == chatId);
        if (user is not null)
        {
            foreach (Url url in user.UrlsToVacancies.Keys)
            {
                await _telegramBotClient.SendTextMessageAsync(chatId, 
                                                              url.OriginalString, 
                                                              disableWebPagePreview: true,
                                                              cancellationToken: cancellationToken);
            }
        }      
    }
}
