using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Soundmates.Api.Common.Filters;

internal sealed class ValidationFilter<TRequest>(IValidator<TRequest> validator) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().FirstOrDefault();
        if (request is null)
            return await next(context);

        var result = await validator.ValidateAsync(request);

        if (!result.IsValid)
            return TypedResults.UnprocessableEntity(new ValidationProblemDetails(result.ToDictionary()));

        return await next(context);
    }
}
