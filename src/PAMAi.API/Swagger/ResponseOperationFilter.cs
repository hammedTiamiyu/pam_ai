using System.Text.Json;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PAMAi.API.Swagger;

/// <summary>
/// Operation filter to modify the responses of operations.
/// </summary>
internal sealed class ResponseOperationFilter: IOperationFilter
{
    #region Problem details
    private static readonly ProblemDetails _problemDetails = new()
    {
        Type = "https://example.com/probs/out-of-credit",
        Title = "You do not have enough credit",
        Detail = "Your current balance is 30, but that costs 50.",
        Instance = "/account/12345/msgs/abc",
        Status = StatusCodes.Status500InternalServerError,
    };
    private static readonly ProblemDetails _notFoundDetails = new()
    {
        Type = "https://example.com/probs/not-found",
        Title = "Not found.",
        Detail = "ID 368e834c-bf9f-4381-870c-b5d64d3a99bd does not match any product in the catalog.",
        Instance = "/products/368e834c-bf9f-4381-870c-b5d64d3a99bd",
        Status = StatusCodes.Status404NotFound,
    };
    private static readonly Dictionary<string, string[]> _errors = new()
    {
        { "CustomerName", new string[] { "Customer name cannot be null or empty." } }
    };
    private static readonly ValidationProblemDetails _validationProblemDetails = new(_errors)
    {
        Type = "https://example.com/probs/invalid-input",
        Title = "Invalid input.",
        Detail = null,
        Instance = "/account",
        Status = StatusCodes.Status400BadRequest,
    };
    #endregion

    #region Open API schemas
    private static readonly OpenApiSchema _problemDetailsSchema = new()
    {
        Type = typeof(ProblemDetails).ToString(),
        Example = new OpenApiString(JsonSerializer.Serialize(_problemDetails)),
    };
    private static readonly OpenApiSchema _notFoundProblemDetailsSchema = new()
    {
        Type = typeof(ProblemDetails).ToString(),
        Example = new OpenApiString(JsonSerializer.Serialize(_notFoundDetails)),
    };
    private static readonly OpenApiSchema _validationProblemDetailsSchema = new()
    {
        Type = typeof(ValidationProblemDetails).ToString(),
        Example = new OpenApiString(JsonSerializer.Serialize(_validationProblemDetails)),
    };
    #endregion

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Remove the default 400 response and add a better one.
        if (operation.Responses.ContainsKey("400"))
        {
            operation.Responses.Remove("400");
            operation.Responses.Add("400", new OpenApiResponse
            {
                Description = "Bad Request",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    {
                        "application/json",
                        new OpenApiMediaType() { Schema = _validationProblemDetailsSchema }
                    },
                }
            });
        }

        // Remove the default 404 response and add a better one.
        if (operation.Responses.ContainsKey("404"))
        {
            operation.Responses.Remove("404");
            operation.Responses.Add("404", new OpenApiResponse
            {
                Description = "Bad Request",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    {
                        "application/json",
                        new OpenApiMediaType() { Schema = _notFoundProblemDetailsSchema }
                    },
                }
            });
        }

        // Remove the default 500 response if it's already added.
        if (operation.Responses.ContainsKey("500"))
            operation.Responses.Remove("500");

        operation.Responses.Add("500", new OpenApiResponse
        {
            Description = "Internal Server Error",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                {
                    "application/json",
                    new OpenApiMediaType() { Schema = _problemDetailsSchema }
                },
            }
        });
    }
}
