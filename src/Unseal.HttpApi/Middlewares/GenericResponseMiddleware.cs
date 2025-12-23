using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
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

            var statusCode = context.Response.StatusCode;

            responseBody.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(responseBody).ReadToEndAsync();
            
            object genericResponse;
            string newResponseContent;

            if (statusCode >= 200 && statusCode < 300)
            {
                var data = string.IsNullOrWhiteSpace(responseText) 
                           ? null 
                           : JsonSerializer
                               .Deserialize<object>(responseText, 
                                   new JsonSerializerOptions
                                   {
                                       PropertyNameCaseInsensitive = true
                                   });
                           
                genericResponse = new
                {
                    Success = true,
                    Data = data,
                    Message = _localizer[ExceptionCodes.Success],
                    StatusCode = statusCode
                };
            }
            else
            {
                var errorDetails = new { 
                    Code = statusCode.ToString(), 
                    Message = _localizer[ExceptionCodes.UnexpectedException],
                    Details = (object?)null
                };

                if (!string.IsNullOrWhiteSpace(responseText))
                {
                     try
                     {
                         var abpError = JsonSerializer.Deserialize<JsonElement>(responseText);
                         if (abpError.TryGetProperty(GenericConstants.Error, out var errorElement))
                         {
                            errorDetails = new {
                                Code = errorElement.GetProperty(GenericConstants.Code).GetString() ?? statusCode.ToString(),
                                Message = _localizer[errorElement.GetProperty(GenericConstants.Message).GetString()!],
                                Details = (object?)errorElement.GetProperty(GenericConstants.Details).GetString()
                            };
                         }
                     }
                     catch
                     {
                         // ignored
                     }
                }

                genericResponse = new
                {
                    Success = false,
                    Data = (object?)null,
                    Message = errorDetails.Message,
                    ErrorDetails = errorDetails,
                    StatusCode = statusCode
                };
            }

            newResponseContent = JsonSerializer.Serialize(genericResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            context.Response.ContentLength = Encoding.UTF8.GetByteCount(newResponseContent);
            context.Response.ContentType = GenericConstants.ContentType;

            await originalBodyStream.WriteAsync(Encoding.UTF8.GetBytes(newResponseContent));
        }
        catch (Exception ex)
        {
            context.Response.Body = originalBodyStream;
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(ExceptionCodes.UnexpectedException);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }
}