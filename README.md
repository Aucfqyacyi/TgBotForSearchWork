# TgBotForSearchWork

## About
This is a telegram bot for search the work on sites: Dou, Djinni, and WorkUa. It tracks them, and if a new vacancy appears, the bot sends it. To start receiving vacancies, you need to call one of the */add_url* or */create_url* commands, which will create a url to the vacancies. 

Firstly bot parses filters from the sites for the url builder with which you can interact with */create_url*, */add_filter*, and */remove_filter* command. Url builder represents filters as they looks in the sites, as a result you can make requests as if you are in the site and searching the jobs. Then every time after timeout, it iterates with users, and if one of them contains the urls, bot starts parsing them on vacancies. If there are new vacancies, they will be sent to user.

Two external libraries are used here, one is mine [AutoDIInjector](https://github.com/Aucfqyacyi/AutoDIInjector) auto registers the services to DI, and another is [botf](https://github.com/deploy-f/botf) from GitHub gives the way to create telegram bots as asp net applications, it has a nuget package but I didn't use, because needed to write some corrections in the library code.


You can test it [BotLink](https://t.me/ForSearchWorkBot) if I didn't off-bot. **Note** bot commands description in the Ukraine language.


## Usage

You have to install MongoDB, the connection string and database name can be changed in appsettings.json. Also, you have to put the bot token here, timeout between sending the vacancies is 60 seconds by default.

``{``

``"botf": "your_token",``  
``"TimeoutBetweenSendVacancies": "time_in_seconds"`` 
  
``}``

That's not all because site Dou is using Cloudflare protection, so before starting the bot, you need to run the [SimpleCloudflareBypass](https://github.com/Aucfqyacyi/SimpleCloudflareBypass) to parse filters from site.