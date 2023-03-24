using AngleSharp;
using AngleSharp.Dom;

namespace Parsers.Extensions;

internal static class HttpClientExtension
{
    public static async ValueTask<TResult> GetElementsAsync<TResult>(this HttpClient httpClient, Uri uri, Func<IDocument, TResult> func, CancellationToken cancellationToken)
    {
        using Stream response = await httpClient.GetStreamIfSuccessAsync(uri, cancellationToken);
        using IBrowsingContext browsingContext = BrowsingContext.New();
        using IDocument document = await browsingContext.OpenAsync(req => req.Content(response), cancellationToken);
        return func(document);
    }

    public static async ValueTask<Stream> GetStreamIfSuccessAsync(this HttpClient httpClient, Uri uri, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(uri, cancellationToken);
        if (response.IsSuccessStatusCode is false)
            throw new Exception($"StatusCode is unsuccessful, error message - {await response.Content.ReadAsStringAsync()}.");
        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }
}
