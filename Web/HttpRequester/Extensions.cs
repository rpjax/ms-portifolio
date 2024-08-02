namespace Aidan.Web.Http;

public static class HttpRequestMessageExtensions
{
    public static HttpMethod GetMethod(this HttpRequestMessage message)
    {
        var x = message.Method.Method;

        switch (x)
        {
            case "GET":
                return HttpMethod.Get;
            case "POST":
                return HttpMethod.Post;
            case "PUT":
                return HttpMethod.Put;
            case "PATCH":
                return HttpMethod.Patch;
            case "DELETE":
                return HttpMethod.Delete;
            case "OPTIONS":
                return HttpMethod.Options;
            default:
                throw new Exception("Could not convert System.Net.Http.HttpMethod to Climatiza.Http.HttpMethod.");
        }
    }
}

public static class HttpResponseMessageExtensions
{
    public static HttpResponse ToHttpResponse(this HttpResponseMessage message, HttpRequest request)
    {
        return new HttpResponse(request, message);
    }

    public static HttpMethod GetMethod(this HttpResponseMessage message)
    {
        if (message.RequestMessage == null)
        {
            throw new NullReferenceException();
        }

        return message.RequestMessage.GetMethod();
    }
}