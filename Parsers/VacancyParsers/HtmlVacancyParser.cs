using AngleSharp.Dom;
using Parsers.Extensions;
using Parsers.Models;
using Parsers.Utilities;

namespace Parsers.VacancyParsers;

internal abstract class HtmlVacancyParser : IVacancyParser
{
    protected abstract HtmlElement VacancyItem { get; }
    protected abstract HtmlElement Title { get; }
    protected abstract HtmlElement Description { get; }
    protected abstract HtmlElement Url { get; }
    protected abstract uint IdPositionInUrl { get; }
    protected abstract string SymbolNearId { get; }

    public async ValueTask<bool> IsCorrectUriAsync(Uri uri, CancellationToken cancellationToken)
    {
        IHtmlCollection<IElement> vacancyElements = await GetVacancyElementsAsync(uri, cancellationToken);
        return vacancyElements.Length > 0;
    }

    public async ValueTask<List<Vacancy>> ParseAsync(Uri uri, int descriptionLenght, IList<ulong>? vacancyIdsToIgnore = null, CancellationToken cancellationToken = default)
    {
        IHtmlCollection<IElement> vacancyElements = await GetVacancyElementsAsync(uri, cancellationToken);
        List<Vacancy> vacancies = new();
        foreach (IElement vacancyElement in vacancyElements)
        {
            Vacancy vacancy = await CreateVacancyAsync(vacancyElement, descriptionLenght, uri.Host, cancellationToken);
            if (vacancyIdsToIgnore is not null && vacancyIdsToIgnore.Contains(vacancy.Id))
                return vacancies;
            else
                vacancies.Add(vacancy);
        }
        return vacancies;
    }

    protected ValueTask<IHtmlCollection<IElement>> GetVacancyElementsAsync(Uri uri, CancellationToken cancellationToken)
    {
        return HtmlParser.GetElementsAsync(uri, document => document.GetElementsByClassName(VacancyItem.CssClassName), cancellationToken);
    }

    protected virtual async ValueTask<Vacancy> CreateVacancyAsync(IElement element, int descriptionLenght, string host, CancellationToken cancellationToken)
    {
        string? url = element.GetElement(Url)?.GetHrefAttribute(host);
        if (url.IsNullOrEmpty())
            throw new Exception($"Url can't be null or empty.");
        string title = element.GetTextContent(Title);
        string description = string.Empty;
        if (descriptionLenght != 0)
            description = await GetDescriptionAsync(url!, descriptionLenght, cancellationToken) ?? string.Empty;
        ulong id = url!.GetNumberFromUrl(IdPositionInUrl, SymbolNearId);
        return new(id, title, url!, description);
    }

    protected virtual async ValueTask<string?> GetDescriptionAsync(string url, int descriptionLenght, CancellationToken cancellationToken)
    {
        var elements = await HtmlParser.GetElementsAsync(new(url), document => document.GetElementsByClassName(Description.CssClassName), cancellationToken);
        return elements.FirstOrDefault()?.GetTextContent(descriptionLenght);
    }
}
