namespace Aidan.Web.Http;

public static class HttpClientSingleton
{
    public static HttpClient Value { get; set; } = new HttpClient();
}

/// <summary>
/// HTTP facade.
/// </summary>
[Obsolete("")]
public class HttpRequester : IDisposable
{
    public static HttpRequester Singleton { get; set; }

    protected HttpClient Client { get; }

    static HttpRequester()
    {
        Singleton = new HttpRequester();
    }

    public HttpRequester()
    {
        Client = new HttpClient();
    }

    public void Dispose()
    {
        Client.Dispose();
    }

    public async Task<HttpResponse> SendAsync(HttpRequest request)
    {
        using var requestMessage = request.ToHttpRequestMessage();
        var responseMessage = await Client.SendAsync(requestMessage);
        return responseMessage.ToHttpResponse(request);
    }
}