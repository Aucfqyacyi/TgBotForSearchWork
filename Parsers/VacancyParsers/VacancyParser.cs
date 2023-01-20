using AngleSharp;
using AngleSharp.Dom;
using Parsers.Constants;
using Parsers.Extensions;
using Parsers.Models;
using System.IO;

namespace Parsers.VacancyParsers;

internal abstract class VacancyParser : IVacancyParser
{
    protected abstract HtmlElement VacancyItem { get; }
    protected abstract HtmlElement Title { get; }
    protected abstract HtmlElement ShortDescription { get; }
    protected abstract HtmlElement Url { get; }
    protected abstract uint IdPositionInUrl { get; }
    protected abstract char SymbolAfterId { get; }


    public virtual async Task<List<Vacancy>> ParseAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        using Stream response = await GlobalHttpClient.GetAsync(uri, cancellationToken);
        using IBrowsingContext browsingContext = BrowsingContext.New();
        using IDocument document = await browsingContext.OpenAsync(req => req.Content(response), cancellationToken);
        IHtmlCollection<IElement> vacancyElements = document.GetElementsByClassName(VacancyItem.CssClassName);
        List<Vacancy> vacancies = new();
        foreach (IElement vacancyElement in vacancyElements)
        {
            vacancies.Add(CreateVacancy(vacancyElement, uri.Host));
        }
        return vacancies;
    }

    private Vacancy CreateVacancy(IElement element, string host)
    {
        string url = element.GetElement(Url)?.GetHrefAttribute(host) ?? string.Empty;
        string title = element.GetTextContent(Title);
        string shortDescription = element.GetTextContent(ShortDescription);
        ulong id = GetId(url);
        return new(id, title, url, shortDescription);
    }

    private Task<Vacancy> CreateVacancyAsync(IElement element, string host)
    {
        return Task.Run(() => CreateVacancy(element, host));
    }

    protected ulong GetId(string url)
    {
        return ulong.Parse(url.Split('/')[IdPositionInUrl].Split(SymbolAfterId).First());       
    }
}
