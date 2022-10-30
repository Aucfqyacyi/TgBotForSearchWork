using AngleSharp.Dom;
using TgBotForSearchWork.Extensions;
using TgBotForSearchWork.Models;
using TgBotForSearchWork.Others;

namespace TgBotForSearchWork.VacancyParsers;

internal abstract class VacancyParser : IVacancyParser
{
    protected abstract HtmlElement VacancyItem { get; }
    protected abstract HtmlElement Title { get; }
    protected abstract HtmlElement ShortDescription { get; }
    protected abstract HtmlElement Url { get; }

    public virtual async Task<List<Vacancy>> ParseAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        using Stream response = await GHttpClient.GetAsync(uri, cancellationToken);
        using IDocument doc = await HtmlDocument.CreateAsync(response, cancellationToken);
        IHtmlCollection<IElement> vacancyElements = doc.GetElementsByClassName(VacancyItem.CssClassName);
        List<Vacancy> vacancies = new();
        foreach (IElement vacancyElement in vacancyElements)
        {
            vacancies.Add(CreateVacancy(vacancyElement, uri.Host));
        }          
        return vacancies;
    }

    private Vacancy CreateVacancy(IElement element, string host)
    {
        string url = element.GetIElement(Url)?.GetHrefAttribute(host) ?? string.Empty;
        string title = element.GetTextContent(Title);
        string shortDescription = element.GetTextContent(ShortDescription);
        return new(title, url, shortDescription);
    }

    private Task<Vacancy> CreateVacancyAsync(IElement element, string host)
    {
        return Task.Run(() => CreateVacancy(element, host));
    }
}
