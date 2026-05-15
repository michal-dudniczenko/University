namespace Soundmates.Api.Common.Validation;

internal static class PaginationValidator
{
    public static IDictionary<string, string[]>? ValidateLimitOffset(int limit, int offset, int maxLimit)
    {
        var errorsBuilder = new Dictionary<string, List<string>>();

        if (limit <= 0)
        {
            AddError(errorsBuilder, "Limit", "Limit parameter value has to be greater than zero.");
        }
        else if (limit > maxLimit)
        {
            AddError(errorsBuilder, "Limit", $"Limit parameter value cannot be greater than {maxLimit}.");
        }

        if (offset < 0)
        {
            AddError(errorsBuilder, "Offset", "Offset parameter value cannot be negative.");
        }

        return errorsBuilder.Count == 0
            ? null
            : errorsBuilder.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
    }

    private static void AddError(
        Dictionary<string, List<string>> errorsBuilder,
        string field,
        string message)
    {
        if (!errorsBuilder.TryGetValue(field, out var errorList))
        {
            errorList = [];
            errorsBuilder[field] = errorList;
        }
        errorList.Add(message);
    }
}
