﻿using Microsoft.AspNetCore.Http;
using Aidan.Core;
using Aidan.Core.Logging;

namespace Aidan.Web;

internal class ExceptionHandlerMiddleware : Middleware
{
    private IErrorLogger? Logger { get; }

    public ExceptionHandlerMiddleware(RequestDelegate next, IErrorLogger? logger) : base(next)
    {
        Logger = logger;
    }

    protected override async Task<Strategy> OnExceptionAsync(HttpContext context, Exception exception)
    {
        if(Logger is not null)
        {
            Logger.Log(Error.FromException(exception));
        }

        await context.WriteProblemResponseAsync(
            statusCode: 500,
            title: "An unexpected error occurred.",
            detail: "The server encountered an unexpected error while processing the request.");

        return Strategy.Break;
    }

}
