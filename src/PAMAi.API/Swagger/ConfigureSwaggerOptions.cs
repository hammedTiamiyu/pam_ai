using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PAMAi.API.Swagger;

/// <summary>
/// Used to configure the <see cref="SwaggerGenOptions"/>.
/// </summary>
internal sealed class ConfigureSwaggerOptions: IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(string? name, SwaggerGenOptions options)
    {
        Configure(options);
    }

    public void Configure(SwaggerGenOptions options)
    {
        // Add swagger document for every API version discovered.
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateVersionInfo(description));
        }

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "Bearer Authentication with JWT Token",
            Type = SecuritySchemeType.Http
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List <string> ()
        }});
        options.OperationFilter<AuthorisationResponseOperationFilter>();
        options.OperationFilter<ResponseOperationFilter>();

        string apiAssembly = typeof(ConfigureSwaggerOptions).Assembly.GetName().Name!;
        string applicationAssembly = typeof(Result).Assembly.GetName().Name!;
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{apiAssembly}.xml"), true);
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{applicationAssembly}.xml"), false);
    }

    private static OpenApiInfo CreateVersionInfo(ApiVersionDescription description)
    {
        var info = new OpenApiInfo
        {
            Title = "Process Flow Manager Web API",
            Version = description.ApiVersion.ToString(),
            Contact = new OpenApiContact
            {
                Name = "LawPavilion Business Solutions",
                Url = new Uri("https://lawpavilion.com"),
            },
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated. Please use one of the newer API versions available from the explorer.";
        }

        return info;
    }
}
