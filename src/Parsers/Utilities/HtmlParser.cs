using AngleSharp;
using AngleSharp.Dom;

namespace Parsers.Utilities;

internal static class HtmlParser
{
    public static async ValueTask<TResult> GetElementsAsync<TResult>(Uri uri, Func<IDocument, TResult> func, CancellationToken cancellationToken)
    {
        using Stream response = await GlobalHttpClient.GetAsync(uri, cancellationToken);
        using IBrowsingContext browsingContext = BrowsingContext.New();
        using IDocument document = await browsingContext.OpenAsync(req => req.Content(response), cancellationToken);
        return func(document);
    }
}
