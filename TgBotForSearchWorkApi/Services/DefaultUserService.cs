﻿using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Repositories;
using TgBotForSearchWorkApi.Utilities.Attributes;

namespace TgBotForSearchWorkApi.Services;

[SingletonService]
public class DefaultUserService
{
    private readonly UserRepository _userRepository;
    private readonly UriToVacanciesRepository _uriToVacanciesRepository;

    public DefaultUserService(UriToVacanciesRepository uriToVacanciesRepository, UserRepository userRepository)
    {
        _uriToVacanciesRepository = uriToVacanciesRepository;
        _userRepository = userRepository;
    }

    public void AddDefaultUser()
    {
        long chatId = 692816611;
        User user = _userRepository.Add(chatId, default);
        if (user.UrlIds.Count > 0)
            return;
        List<UriToVacancies> uriToVacancies = new()
        {
            new(user.Id, "https://jobs.dou.ua/vacancies/?remote&category=.NET&exp=1-3"),
            new(user.Id, "https://jobs.dou.ua/vacancies/?remote&category=C%2B%2B&exp=1-3"),
            new(user.Id, "https://jobs.dou.ua/vacancies/?remote&category=C%2B%2B&exp=0-1"),
            new(user.Id, "https://djinni.co/jobs/?employment=remote&primary_keyword=C%2B%2B&exp_level=no_exp&exp_level=1y"),
            new(user.Id, "https://djinni.co/jobs/?primary_keyword=.NET&exp_level=1y&exp_level=2y&employment=remote"),
            new(user.Id, "https://www.work.ua/ru/jobs-remote-c%2B%2B+developer/")
        };
        user.UrlIds.AddRange(uriToVacancies.Select(e => e.Id));
        _uriToVacanciesRepository.InsertMany(uriToVacancies, default);
        _userRepository.Replace(user, default);
    }
}