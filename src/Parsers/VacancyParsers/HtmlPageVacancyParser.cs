using AngleSharp.Dom;
using Parsers.Extensions;
using Parsers.Models;

namespace Parsers.VacancyParsers;

internal abstract class HtmlPageVacancyParser : IVacancyParser
{
    protected readonly HttpClient _httpClient;

    protected abstract HtmlElement VacancyItem { get; }
    protected abstract HtmlElement Title { get; }
    protected abstract HtmlElement Description { get; }
    protected abstract HtmlElement Url { get; }
    protected abstract uint IdPositionInUrl { get; }
    protected abstract string SymbolNearId { get; }

    public HtmlPageVacancyParser(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async ValueTask<List<Vacancy>> ParseAsync(Uri uri, uint descriptionLenght, IReadOnlyList<ulong>? vacancyIdsToIgnore = null, CancellationToken cancellationToken = default)
    {
        var vacancyListElement = (await GetVacancyElementsAsync(uri, cancellationToken)).FirstOrDefault();
		List<Vacancy> vacancies = new();
        IElement? vacancyElement = vacancyListElement?.FirstElementChild;
        if (vacancyElement is null)
            return new();
        do
        {
            (string url, ulong id) = GetVacancyUrlAndId(vacancyElement, uri.Host);
            if (vacancyIdsToIgnore is not null && vacancyIdsToIgnore.Contains(id))
                return vacancies;
            Vacancy vacancy = await CreateVacancyAsync(vacancyElement, url, id, descriptionLenght, cancellationToken);
            vacancies.Add(vacancy);
        } while ((vacancyElement = vacancyElement?.NextElementSibling) is not null);

		return vacancies;
    }

    public async ValueTask<bool> IsCorrectUriAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        try
        {
            IHtmlCollection<IElement> vacancyElements = await GetVacancyElementsAsync(uri, cancellationToken);
            return vacancyElements.Length > 0;
        }
        catch
        {
            return false;
        }
    }

    protected ValueTask<IHtmlCollection<IElement>> GetVacancyElementsAsync(Uri uri, CancellationToken cancellationToken)
    {
        return _httpClient.GetElementsAsync(uri, document => document.GetElementsByClassName(VacancyItem.CssClassName), cancellationToken);
    }

    protected async ValueTask<Vacancy> CreateVacancyAsync(IElement element, string url, ulong id, uint descriptionLenght, CancellationToken cancellationToken)
    {
        string title = element.GetTextContent(Title);
        string description = string.Empty;
        if (descriptionLenght != 0)
            description = await GetDescriptionAsync(url!, descriptionLenght, cancellationToken) ?? string.Empty;
        return new Vacancy(id, title, url!, description);
    }

    protected (string url, ulong id) GetVacancyUrlAndId(IElement element, string host)
    {
        string? url = element.GetElement(Url)?.GetHrefAttribute(host);
        if (url.IsNullOrEmpty())
            throw new Exception($"Url can't be null or empty.");
        ulong id = url!.GetNumberFromUrl(IdPositionInUrl, SymbolNearId);
        return (url!, id);
    }

    protected virtual async ValueTask<string?> GetDescriptionAsync(string url, uint descriptionLenght, CancellationToken cancellationToken)
    {
        var elements = await _httpClient.GetElementsAsync(new(url), document => document.GetElementsByClassName(Description.CssClassName), cancellationToken);
        return elements.FirstOrDefault()?.GetTextContent(descriptionLenght);
    }
}
