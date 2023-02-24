using AngleSharp.Dom;
using Parsers.Extensions;
using Parsers.Models;
using Parsers.Utilities;

namespace Parsers.VacancyParsers;

internal abstract class VacancyParser : IVacancyParser
{
    protected abstract HtmlElement VacancyItem { get; }
    protected abstract HtmlElement Title { get; }
    protected abstract HtmlElement Description { get; }
    protected abstract HtmlElement Url { get; }
    protected abstract uint IdPositionInUrl { get; }
    protected abstract string SymbolNearId { get; }

    public async ValueTask<bool> IsCorrectUrlAsync(Uri uri, CancellationToken cancellationToken)
    {
        IHtmlCollection<IElement> vacancyElements = await GetVacancyElementsAsync(uri, cancellationToken);
        return vacancyElements.Length > 0;
    }

    public async ValueTask<List<Vacancy>> ParseAsync(Uri uri, int descriptionLenght, CancellationToken cancellationToken = default)
    {
        IHtmlCollection<IElement> vacancyElements = await GetVacancyElementsAsync(uri, cancellationToken);
        List<Vacancy> vacancies = new();
        foreach (IElement vacancyElement in vacancyElements)
        {
            vacancies.Add(await CreateVacancyAsync(vacancyElement, descriptionLenght, uri.Host, cancellationToken));
        }
        return vacancies;
    }

    protected ValueTask<IHtmlCollection<IElement>> GetVacancyElementsAsync(Uri uri, CancellationToken cancellationToken)
    {        
        return HtmlParser.GetElementsAsync(uri, document => document.GetElementsByClassName(VacancyItem.CssClassName), cancellationToken);
    }

    protected virtual async ValueTask<Vacancy> CreateVacancyAsync(IElement element, int descriptionLenght, string host, CancellationToken cancellationToken)
    {
        string url = element.GetElement(Url)?.GetHrefAttribute(host) ?? string.Empty;
        string title = element.GetTextContent(Title);
        string description = await GetDescriptionAsync(url, descriptionLenght, cancellationToken) ?? string.Empty;
        ulong id = GetId(url);
        return new(id, title, url, description);
    }

    protected virtual async ValueTask<string?> GetDescriptionAsync(string url, int descriptionLenght, CancellationToken cancellationToken)
    {
        var elements = await HtmlParser.GetElementsAsync(new(url), document => document.GetElementsByClassName(Description.CssClassName), cancellationToken);
        return elements.FirstOrDefault()?.GetTextContent(descriptionLenght);
    }

    protected ulong GetId(string url)
    {
        string rawId = url.Split('/')[IdPositionInUrl];
        string[] splitedRawId = rawId.Split(SymbolNearId);
        string id = splitedRawId.First();
        if (id.IsNullOrEmpty())
            id = splitedRawId.Last();
        return ulong.Parse(id);       
    }
}
