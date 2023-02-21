﻿using SimpleCloudflareBypass.Models;

namespace SimpleCloudflareBypass.Utilities;

public class EndpointFilter : IEndpointFilter
{   
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            SendManyRequest? manyRequest = context.Arguments.First() as SendManyRequest;
            if (manyRequest is null)
            {
                SendRequest? request = context.Arguments.First() as SendRequest;
                CheckRequest(request);
            }
            else
            {
                foreach (var request in manyRequest.Requests)
                {
                    CheckRequest(request);
                }
            }            
        }
        catch(ArgumentNullException ex)
        {
            LogErrorMessage(ex.Message);
            return Results.BadRequest(new ErrorResponse("Request url is empty."));
        }
        catch(Exception ex)
        {
            LogErrorMessage(ex.Message);
            return Results.BadRequest(new ErrorResponse("Request is not correct."));
        }
        return await next(context);
    }

    private void CheckRequest(SendRequest? request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentNullException.ThrowIfNull(request.Url, nameof(request.Url));
        bool isCorrectUrl = Uri.TryCreate(request.Url, UriKind.Absolute, out Uri? uri);
        if (isCorrectUrl is false)
            throw new Exception();
    }

    private void LogErrorMessage(string message)
    {
        Console.WriteLine($"In EndpointFilter was throwed exception, error = {message}");
    }
}
