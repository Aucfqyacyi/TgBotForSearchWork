﻿using AngleSharp;
using AngleSharp.Dom;
using Parsers.Extensions;
using Parsers.Models;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Parsers.VacancyParsers.Parsers;

internal class DouVacancyParser : IVacancyParser
{

    public async ValueTask<List<Vacancy>> ParseAsync(Uri uri, int descriptionLenght, IReadOnlyList<ulong>? vacancyIdsToIgnore = null, CancellationToken cancellationToken = default)
    {
        SyndicationFeed feed = GetSyndicationFeed(uri);
        List<Vacancy> vacancies = new();
        foreach (SyndicationItem item in feed.Items)
        {
            Vacancy vacancy = await CreateVacancyAsync(item, descriptionLenght, cancellationToken);
            if (vacancyIdsToIgnore is not null && vacancyIdsToIgnore.Contains(vacancy.Id))
                return vacancies;
            else
                vacancies.Add(vacancy);
        }
        return vacancies;
    }

    public ValueTask<bool> IsCorrectUriAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        try
        {
            SyndicationFeed feed = GetSyndicationFeed(uri);
            if (feed.Items.TryGetNonEnumeratedCount(out int count))
                return ValueTask.FromResult(count > 0);
            else
                return ValueTask.FromResult(feed.Items.Count() > 0);
        }
        catch (Exception)
        {
            return ValueTask.FromResult(false);
        }        
    }

    private Uri AddFeedsStrToUri(Uri uri)
    {
        string feeds = "feeds/";
        if (uri.LocalPath.EndsWith(feeds) is true)
            return uri;
        UriBuilder uriBuilder = new(uri);
        uriBuilder.Path = uri.LocalPath + feeds;
        return uriBuilder.Uri;
    }

    protected SyndicationFeed GetSyndicationFeed(Uri uri)
    {
        using XmlReader reader = XmlReader.Create(AddFeedsStrToUri(uri).OriginalString, new() { DtdProcessing  = DtdProcessing.Ignore, });
        return SyndicationFeed.Load(reader);
    }

    protected async ValueTask<Vacancy> CreateVacancyAsync(SyndicationItem item, int descriptionLenght, CancellationToken cancellationToken)
    {
        string url = item.Id;
        string title = item.Title.Text.ParseHtml();
        string description = await GetDescriptionAsync(item, descriptionLenght, cancellationToken);
        ulong id = url!.GetNumberFromUrl(6, "/");
        return new Vacancy(id, title, url, description);
    }

    protected async ValueTask<string> GetDescriptionAsync(SyndicationItem item, int descriptionLenght, CancellationToken cancellationToken)
    {
        using IBrowsingContext browsingContext = BrowsingContext.New();
        using IDocument document = await browsingContext.OpenAsync(req => req.Content(item.Summary.Text), cancellationToken);
        return document.GetElementsByClassName("text __r").First().GetTextContent(descriptionLenght);
    }
}