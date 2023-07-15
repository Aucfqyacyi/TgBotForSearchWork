using AngleSharp;
using AngleSharp.Dom;
using Parsers.Extensions;
using Parsers.Models;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Parsers.VacancyParsers.Parsers;

internal class DouVacancyParser : IVacancyParser
{

    public async ValueTask<List<Vacancy>> ParseAsync(Uri uri, uint descriptionLenght, IReadOnlyList<ulong>? vacancyIdsToIgnore = null, CancellationToken cancellationToken = default)
    {
        SyndicationFeed feed = GetSyndicationFeed(uri);
        List<Vacancy> vacancies = new();
        foreach (SyndicationItem item in feed.Items)
        {
            ulong vacancyId = item.Id.GetNumberFromUrl(6, "/");
            if (vacancyIdsToIgnore is not null && vacancyIdsToIgnore.Contains(vacancyId))
                return vacancies;
            Vacancy vacancy = await CreateVacancyAsync(vacancyId, item, descriptionLenght, cancellationToken);
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
        if (uri.LocalPath.EndsWith(feeds))
            return uri;
        UriBuilder uriBuilder = new (uri)
        {
            Path = uri.LocalPath + feeds
        };
        return uriBuilder.Uri;
    }

    protected SyndicationFeed GetSyndicationFeed(Uri uri)
    {
        using XmlReader reader = XmlReader.Create(AddFeedsStrToUri(uri).OriginalString, new() { DtdProcessing = DtdProcessing.Ignore });
        return SyndicationFeed.Load(reader);
    }

    protected async ValueTask<Vacancy> CreateVacancyAsync(ulong id, SyndicationItem item, uint descriptionLenght, CancellationToken cancellationToken)
    {
        string url = item.Id;
        string title = item.Title.Text.ParseHtml();
        string description = await GetDescriptionAsync(item, descriptionLenght, cancellationToken);
        return new Vacancy(id, title, url, description);
    }


    protected async ValueTask<string> GetDescriptionAsync(SyndicationItem item, uint descriptionLenght, CancellationToken cancellationToken)
    {
        using IBrowsingContext browsingContext = BrowsingContext.New();
        using IDocument document = await browsingContext.OpenAsync(req => req.Content(item.Summary.Text), cancellationToken);
        return document.GetElementsByClassName("text __r").First().GetTextContent(descriptionLenght);
    }
}
