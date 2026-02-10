using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Localization;

public class GenericResponseMiddleware
{
    private readonly IStringLocalizer<UnsealResource> _localizer;
    private readonly RequestDelegate _next;

    public GenericResponseMiddleware(RequestDelegate next, IStringLocalizer<UnsealResource> localizer)
    {
        _next = next;
        _localizer = localizer;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status204NoContent)
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
            }

            responseBody.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(responseBody).ReadToEndAsync();

            if (context.Response.HasStarted)
            {
                await responseBody.CopyToAsync(originalBodyStream);
                return;
            }

            var statusCode = context.Response.StatusCode;
            bool isSuccess = statusCode is >= 200 and < 300;

            object? data = null;
            object? errorDetails = null;
            string message = string.Empty;

            if (isSuccess)
            {
                if (string.IsNullOrWhiteSpace(responseText))
                {
                    data = null;
                }
                else
                {
                    var trimmedResponse = responseText.Trim();
                    bool isJsonObject = (trimmedResponse.StartsWith("{") && trimmedResponse.EndsWith("}")) || 
                                        (trimmedResponse.StartsWith("[") && trimmedResponse.EndsWith("]"));

                    if (isJsonObject)
                    {
                        using var doc = JsonDocument.Parse(responseText);
                        data = doc.RootElement.Clone();
                    }
                    else
                    {
                        data = responseText;
                    }
                }

                message = _localizer[ExceptionCodes.Success];
            }
            else
            {
                var (code, msg, details) = ParseErrorResponse(responseText, statusCode);
                message = msg;
                errorDetails = new { Code = code, Message = msg, Details = details };
            }

            var genericResponse = new
            {
                Success = isSuccess,
                Data = data,
                Message = message,
                ErrorDetails = errorDetails,
                StatusCode = statusCode
            };

            var newResponseContent = JsonSerializer.Serialize(genericResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            });

            context.Response.ContentType = AppConstants.GenericResponse.ContentType;
            context.Response.ContentLength = Encoding.UTF8.GetByteCount(newResponseContent);

            await originalBodyStream.WriteAsync(Encoding.UTF8.GetBytes(newResponseContent));
        }
        catch (Exception)
        {
            // ignored
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private (string Code, string Message, string? Details) ParseErrorResponse(
        string responseText,
        int statusCode
    )
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(responseText))
            {
                var abpError = JsonSerializer.Deserialize<JsonElement>(responseText);
                if (abpError.TryGetProperty(AppConstants.GenericResponse.Error, out var errorElement))
                {
                    return (
                        errorElement.GetProperty(AppConstants.GenericResponse.Code).GetString() ??
                        statusCode.ToString(),
                        _localizer[errorElement.GetProperty(AppConstants.GenericResponse.Message).GetString() ?? ""],
                        errorElement.TryGetProperty(AppConstants.GenericResponse.Details, out var d)
                            ? d.GetString()
                            : null
                    );
                }
            }
        }
        catch
        {
            // ignored
        }

        return (
            statusCode.ToString(),
            _localizer[ExceptionCodes.UnexpectedException],
            null
        );
    }
}