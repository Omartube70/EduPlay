using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;

namespace API.Extensions
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info.Title = "EduPlay API";
                    document.Info.Version = "v1";
                    document.Info.Description = "Upload documents, extract text, and generate AI summaries.";

                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header                    };

                    return Task.CompletedTask;
                });

                options.AddOperationTransformer((operation, context, cancellationToken) =>
                {
                    var requiresAuth = context.Description.ActionDescriptor.EndpointMetadata
                        .OfType<IAuthorizeData>()
                        .Any();

                    if (requiresAuth)
                    {
                        var schemeReference = new OpenApiSecuritySchemeReference("Bearer", context.Document);

                        var requirement = new OpenApiSecurityRequirement();
                        requirement.Add(schemeReference, new List<string>());

                        operation.Security ??= new List<OpenApiSecurityRequirement>();
                        operation.Security.Add(requirement);
                    }

                    return Task.CompletedTask;
                });
            });

            return services;
        }
    }
}