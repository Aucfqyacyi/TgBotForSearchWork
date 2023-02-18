using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Repositories;
using TgBotForSearchWorkApi.Utilities.Attributes;

namespace TgBotForSearchWorkApi.Services;

[SingletonService]
public class DefaultUserService
{
    private readonly UriToVacanciesRepository _uriToVacanciesRepository;

    public DefaultUserService(UriToVacanciesRepository uriToVacanciesRepository)
    {
        _uriToVacanciesRepository = uriToVacanciesRepository;
    }

    public void AddDefaultUser()
    {
        long chatId = 692816611;
        if (_uriToVacanciesRepository.GetAll(chatId, Parsers.Constants.SiteType.Dou, default).Count > 0)
            return;
        List<UriToVacancies> uriToVacancies = new()
        {
            new(chatId, "https://jobs.dou.ua/vacancies/?remote&category=.NET&exp=1-3", true),
            new(chatId, "https://jobs.dou.ua/vacancies/?remote&category=C%2B%2B&exp=1-3", true),
            new(chatId, "https://jobs.dou.ua/vacancies/?remote&category=C%2B%2B&exp=0-1", true),
            new(chatId, "https://djinni.co/jobs/?employment=remote&primary_keyword=C%2B%2B&exp_level=no_exp&exp_level=1y", true),
            new(chatId, "https://djinni.co/jobs/?primary_keyword=.NET&exp_level=1y&exp_level=2y&employment=remote", true),
            new(chatId, "https://www.work.ua/ru/jobs-remote-c%2B%2B+developer/", true)
        };
        _uriToVacanciesRepository.InsertMany(uriToVacancies, default);
    }
}
