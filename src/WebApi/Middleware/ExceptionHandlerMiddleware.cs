﻿using System.Net;

using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using SolutionTemplate.Application;
using SolutionTemplate.Application._.Behaviors;

using ApplicationException = SolutionTemplate.Application._.Behaviors.ApplicationException;

namespace SolutionTemplate.WebApi.Middleware;

internal sealed class ExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }



    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        httpContext.Response.ContentType = "application/json";

        ProblemDetails problem = new();
        switch (exception)
        {
            case ValidationException validationException:
                _logger.ValidationError(validationException.Errors, exception);

                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                problem.Title = "One or more validation errors occurred.";
                problem.Detail = exception.Message;
                break;
            case ApplicationException applicationException:
                _logger.ApplicationError(applicationException.Code.ToString(), exception);

                httpContext.Response.StatusCode = MapErrorToStatusCode(applicationException.Code);
                problem.Type = nameof(ApplicationException);
                problem.Title = exception.Message;
                break;
            case DomainException domainException:
                _logger.DomainError(domainException.Code, exception);

                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                problem.Type = nameof(DomainException);
                problem.Title = exception.Message;
                break;

            default:
                _logger.UnExpectedError(exception);

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                problem.Title = "An unhandled error occurred.";
                break;
        }

        problem.Status = httpContext.Response.StatusCode;

        var json = JsonConvert.SerializeObject(problem);

        await httpContext.Response.WriteAsync(json);
    }
    private static int MapErrorToStatusCode(ApplicationErrors.Code code)
    {
        return code switch
        {
            ApplicationErrors.Code.NotFound => (int)HttpStatusCode.NotFound,
            ApplicationErrors.Code.AlreadyExists => (int)HttpStatusCode.Conflict,
            _ => throw new NotSupportedException($"Code {code} is not supported.")
        };
    }
}

public static partial class Log
{
    [LoggerMessage(0, LogLevel.Error, "Unexpected exception")]
    public static partial void UnExpectedError(this ILogger logger, Exception exception);


    [LoggerMessage(1, LogLevel.Error, "Validation errors: {errors}")]
    public static partial void ValidationError(this ILogger logger, IEnumerable<ValidationFailure> errors, Exception exception);


    [LoggerMessage(2, LogLevel.Warning, "Application error: {code}")]
    public static partial void ApplicationError(this ILogger logger, string code, Exception exception);


    [LoggerMessage(3, LogLevel.Warning, "Domain error: {code}")]
    public static partial void DomainError(this ILogger logger, string code, Exception exception);
}
