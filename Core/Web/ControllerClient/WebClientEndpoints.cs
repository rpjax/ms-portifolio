using ModularSystem.Core;
using ModularSystem.Web.Http;

namespace ModularSystem.Web.Client;

//*
// IMPLEMENTATIONS.
//*

internal class PingEndpoint : EndpointBase
{
    public PingEndpoint(Http.Uri uri) : base(uri)
    {

    }

    protected override HttpRequest CreateRequest()
    {
        return new HttpRequest(RequestUri.AppendPath("ping"), HttpMethod.Get);
    }
}

// CREATE
internal class CreateEndpoint<T> : EndpointBase<T, Dto<string>> where T : class
{
    public CreateEndpoint(Http.Uri uri) : base(uri)
    {

    }

    protected override HttpRequest CreateRequest(T input)
    {
        return new HttpRequest(RequestUri, HttpMethod.Post, new JsonBody(input));
    }
}

// READ
internal class GetByIdEndpoint<T> : EndpointBase<string, T>
{
    public GetByIdEndpoint(Http.Uri uri) : base(uri)
    {

    }

    protected override HttpRequest CreateRequest(string input)
    {
        return new HttpRequest(RequestUri, HttpMethod.Get)
            .SetQueryParam("id", input);
    }
}

internal class QueryEndpoint<T> : EndpointBase<SerializableQuery, QueryResult<T>>
{
    public QueryEndpoint(Http.Uri uri) : base(uri)
    {

    }

    protected override HttpRequest CreateRequest(SerializableQuery input)
    {
        return new HttpRequest(RequestUri.AppendPath("query"), HttpMethod.Post)
            .SetJsonBody(input);
    }
}

// UPDATE
internal class UpdateEndpoint<T> : EndpointBase<T, Dto<bool>> where T : class
{
    public UpdateEndpoint(Http.Uri uri) : base(uri)
    {

    }

    protected override HttpRequest CreateRequest(T input)
    {
        var body = new JsonBody(input);
        return new HttpRequest(RequestUri, HttpMethod.Put, body);
    }
}

// DELETE
internal class DeleteByIdEndpoint<T> : EndpointBase<string, Core.Void>
{
    public DeleteByIdEndpoint(Http.Uri uri) : base(uri)
    {

    }

    protected override HttpRequest CreateRequest(string input)
    {
        return new HttpRequest(RequestUri, HttpMethod.Delete)
            .SetQueryParam("id", input);
    }

    protected override Core.Void DeserializeResponse(HttpResponse response)
    {
        return new Core.Void();
    }
}

// OTHER APIs
internal class ValidateIdEndpoint<T> : EndpointBase<string, Dto<bool>>
{
    public ValidateIdEndpoint(Http.Uri uri) : base(uri)
    {

    }

    protected override HttpRequest CreateRequest(string input)
    {
        return new HttpRequest(RequestUri.AppendPath("id-validation"), HttpMethod.Get)
            .SetQueryParam("id", input);
    }
}