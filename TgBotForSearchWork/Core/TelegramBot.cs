﻿using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotForSearchWork.Core.Constants;
using TgBotForSearchWork.Core.Other;
using TgBotForSearchWork.Core.UserManagers;
using TgBotForSearchWork.Extensions;
using TgBotForSearchWork.VacancyParsers;
using TgBotForSearchWork.VacancyParsers.AllVacancyParsers;
using TgBotForSearchWork.VacancyParsers.DetailVacancyVarsers;
using TgBotForSearchWork.VacancyParsers.Models;
using User = TgBotForSearchWork.Core.UserManagers.User;

namespace TgBotForSearchWork.Core;

public class TelegramBot
{
	private readonly TelegramBotClient _telegramBotClient;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ReceiverOptions _receiverOptions;
    private readonly UserManager _userManager;
    private readonly TimeSpan _timeOut;

    public TelegramBot(string token, TimeSpan timeOut, UserManager userManager, ReceiverOptions ? receiverOptions = null)
    {
        _telegramBotClient = new(token, GHttpClient.Client);
        receiverOptions ??= new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() };
        _receiverOptions = receiverOptions;
        _userManager = userManager;
        _timeOut = timeOut;
    }

    public async Task Start()
    {
        _telegramBotClient.StartReceiving(
                    updateHandler: HandleUpdateAsync,
                    pollingErrorHandler: HandlePollingErrorAsync,
                    receiverOptions: _receiverOptions,
                    cancellationToken: _cancellationTokenSource.Token);
        try
        {
            while (true)
            {
                foreach (var user in _userManager.Users)
                    await SendVacancy(user, _cancellationTokenSource.Token);
                await Task.Delay(_timeOut, _cancellationTokenSource.Token);
            }
        }
        catch (TaskCanceledException)
        {

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Stop();
        }      
    }

    private async Task SendVacancy(User user, CancellationToken cancellationToken = default)
    {
        foreach (var uriToVacancy in user.UrisToVacancies)
        {
            using Stream response = await GHttpClient.GetAsync(uriToVacancy.Key, cancellationToken);
            List<Vacancy> vacancies = await GetRelevantVacancies(response, uriToVacancy.Key, uriToVacancy.Value, cancellationToken);
            if (vacancies.Count != 0)
            {
                await SendVacancy(user.ChatId, vacancies);
                user.UrisToVacancies[uriToVacancy.Key] = vacancies.FirstOrDefault();
            }
        }
    }

    private async Task SendVacancy(long chatId, List<Vacancy> vacancies, CancellationToken cancellationToken = default)
    {
        foreach (var vacancy in vacancies)
            await SendVacancy(chatId, vacancy, cancellationToken);
    }

    private async Task SendVacancy(long chatId, Vacancy vacancy, CancellationToken cancellationToken = default)
    {
        await _telegramBotClient.SendTextMessageAsync(chatId, 
                                                      vacancy.Present(), 
                                                      ParseMode.Markdown,
                                                      replyMarkup: GetInlineButton(vacancy.Url),
                                                      disableWebPagePreview: true,
                                                      cancellationToken: cancellationToken);
    }

    private async Task SendDetailVacancyDescription(long chatId, int messageId, string url, CancellationToken cancellationToken = default)
    {
        if (url.IsNotNullOrEmpty())
        {
            using Stream response = await GHttpClient.GetAsync(url, cancellationToken);
            IDetailVacancyParser vacancyParser = VacancyParserFactory.CreateDetailVacancyParser(new Uri(url));
            string vacancyDescription = await vacancyParser.ParseAsync(response, cancellationToken);
            await _telegramBotClient.SendTextMessageAsync(chatId,
                                                          vacancyDescription,
                                                          ParseMode.Markdown,
                                                          replyToMessageId: messageId,
                                                          disableWebPagePreview: true,
                                                          cancellationToken: cancellationToken);
        }
        
    }

    private async Task<List<Vacancy>> GetRelevantVacancies(Stream response, Uri uri, Vacancy? lastVacancy, 
                                                                            CancellationToken cancellationToken = default)
    {
        IAllVacancyParser vacancyParser = VacancyParserFactory.CreateAllVacancyParser(uri);
        List<Vacancy> vacancies = await vacancyParser.ParseAsync(response, uri.Host, cancellationToken);
        if (lastVacancy is not null)
        {
            if (lastVacancy == vacancies.FirstOrDefault())
            {
                return new();
            }
            int index = vacancies.FindIndex(vacancy => vacancy.Title == lastVacancy.Title);
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
                case UpdateType.CallbackQuery:
                    await OnCallbackQuery(botClient, update, cancellationToken);
                    break;
            }       
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
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

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }

    private IReplyMarkup GetInlineButton(string url)
    {
        return new InlineKeyboardMarkup(new InlineKeyboardButton(Command.GetFullVacancy) 
                                        { 
                                            CallbackData = UrlEncryption.Encrypt(new Uri(url)) 
                                        });
    }

    private async Task OnMessage(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default)
    {
        string messageText = update.Message!.Text!;
        var chatId = update.Message!.Chat.Id;
        if (Command.Start.Contains(messageText))
            _userManager.AddUser(chatId);
        if (Command.Stop.Contains(messageText))
            _userManager.RemoveUser(chatId);
        if (Command.Test.Contains(messageText))
            await botClient.SendTextMessageAsync(chatId, "Hello, I am friendly neighborhood bot <3.\n", cancellationToken: cancellationToken);
    }

    private async Task OnCallbackQuery(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default)
    {
        var callbackQuery = update.CallbackQuery;
        string data = callbackQuery!.Data!;
        await SendDetailVacancyDescription(callbackQuery!.Message!.Chat.Id, 
                                           callbackQuery.Message!.MessageId, 
                                           UrlEncryption.Dencrypt(data),                          
                                           cancellationToken);
    }
}