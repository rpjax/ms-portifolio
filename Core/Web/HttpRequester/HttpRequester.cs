namespace ModularSystem.Web.Http;

/// <summary>
/// HTTP facade.
/// </summary>
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