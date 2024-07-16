using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PAMAi.API.Swagger;

/// <summary>
/// Swagger operation filter to add 401 and 403 status code responses to operations
/// that require authorisation.
/// </summary>
internal sealed class AuthorisationResponseOperationFilter: IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check if the method or containing controller is decorated with the AuthorizeAttribute.
        var authAttributes = context.MethodInfo
            .GetCustomAttributes(true)
            .Union(context.MethodInfo.DeclaringType?.GetCustomAttributes(true) ?? [])
            .OfType<AuthorizeAttribute>();

        // Check if the method overrides the AuthorizeAttribute with the AllowAnonymousAttribute.
        var allowAnonAttributesOnMethod = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AllowAnonymousAttribute>();

        if (authAttributes.Any() && !allowAnonAttributesOnMethod.Any())
        {
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorised" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
        }
    }
}
