using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Filters;

namespace Soundmates.Api.OpenApiTransformers;

internal sealed class CsrfTokenHeaderTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var hasCsrfAttribute = context.Description.ActionDescriptor.EndpointMetadata
            .OfType<ValidateCsrfTokenAttribute>()
            .Any();

        if (!hasCsrfAttribute) return Task.CompletedTask;

        operation.Parameters ??= [];

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = SecurityConstants.CsrfTokenHeaderName,
            In = ParameterLocation.Header,
            Required = false,
            Description = "Required only for cookie-authenticated requests. Obtain from GET /auth/csrf-token and include in the X-CSRF-TOKEN header.",
            Schema = new OpenApiSchema { Type = JsonSchemaType.String }
        });

        return Task.CompletedTask;
    }
}
