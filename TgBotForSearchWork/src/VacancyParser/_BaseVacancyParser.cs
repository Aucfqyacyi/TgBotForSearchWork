using AngleSharp;
using AngleSharp.Dom;
using System.Collections.Generic;
using System.Xml.Linq;
using TgBotForSearchWork.src.Extensions;
using TgBotForSearchWork.src.Other;
using TgBotForSearchWork.src.TelegramBot.Models;

namespace TgBotForSearchWork.src.VacancyParser;

public abstract class BaseVacancyParser
{
    protected abstract CssClass VacancyItem { get; }
    protected abstract CssClass Title { get; }
    protected abstract CssClass Description { get; }
    protected abstract CssClass Date { get; }
    protected abstract CssClass Url { get; }

    public async Task<List<Vacancy>> ParseAsync(Stream stream, string host, CancellationToken cancellationToken = default)
    {        
        IDocument doc = await HtmlDocument.Create(stream, cancellationToken);
        IHtmlCollection<IElement> vacancyElements = doc.GetElementsByClassName(VacancyItem.Name);
        List<Vacancy> vacancies = new();
        foreach (IElement vacancyElement in vacancyElements)
        {
            IElement? element = vacancyElement.FirstElementChild;           
            if (element is not null)
            {
                vacancies.Add(CreateVacancy(element, host));
            }          
        }
        return vacancies;
    }

    private Vacancy CreateVacancy(IElement element, string host)
    {
        Vacancy vacancy = new();
        do
        {
            IElement nextElement = element;
            do
            {
                AddTitle(vacancy, nextElement, host);
                AddDescription(vacancy, nextElement);
                AddDate(vacancy, nextElement);

            } while ((nextElement = nextElement?.NextElementSibling) is not null);
        } while ((element = element?.FirstElementChild) is not null && string.IsNullOrEmpty(vacancy.Title));
        return vacancy;
    }

    protected virtual void AddTitle(Vacancy vacancy, IElement element, string host)
    {
        if (element.ClassName != Title.Name)
        {
            return;
        }
        IHtmlCollection<IElement> tagAs = element.GetElementsByTagName("a");
        foreach (IElement tagA in tagAs)
        {
            if (tagA.ClassName == Url.Name)
            {
                vacancy.Title = tagA.GetFirstChildInnerHtml();
                vacancy.Url = tagA.GetHref(host);
                break;
            }
        }
            
    }

    protected virtual void AddDescription(Vacancy vacancy, IElement element)
    {
        if (element.ClassName != Description.Name)
        {
            return;
        }
        vacancy.Description = element.GetFirstChildInnerHtml();
    }

    protected virtual void AddDate(Vacancy vacancy, IElement element)
    {
        if (element.ClassName != Date.Name)
        {
            return;
        }
        vacancy.Date = element.GetFirstChildInnerHtml();
    }
}
