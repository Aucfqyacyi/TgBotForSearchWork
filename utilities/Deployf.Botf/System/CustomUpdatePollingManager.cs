﻿using Telegram.Bot;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Deployf.Botf;

public class CustomUpdatePollingManager<TBot> : IUpdatePollingManager<TBot> where TBot : IBot
{
    private readonly UpdateDelegate _updateDelegate;
    private readonly IBotServiceProvider _rootProvider;

    public CustomUpdatePollingManager(IBotBuilder botBuilder, IBotServiceProvider rootProvider)
    {
        _updateDelegate = botBuilder.Build();
        _rootProvider = rootProvider;
    }

    public async Task RunAsync(
      GetUpdatesRequest requestParams,
      CancellationToken cancellationToken = default)
    {
        var bot = (TBot)_rootProvider.GetRequiredService(typeof(TBot));
        await bot.Client.DeleteWebhookAsync(false, cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            var updates = await bot.Client.MakeRequestAsync(requestParams, cancellationToken);

            foreach (var item in updates)
            {
                ProcessUpdate(bot, item, cancellationToken);
            }

            if (updates.Length != 0)
                requestParams.Offset = updates[updates.Length - 1].Id + 1;
        }

        cancellationToken.ThrowIfCancellationRequested();
    }

    async void ProcessUpdate(TBot bot, Update item, CancellationToken cancellationToken)
    {
        using IBotServiceProvider scopeProvider = _rootProvider.CreateScope();
        await _updateDelegate(new UpdateContext(bot, item, scopeProvider), cancellationToken);
    }
}