namespace Soundmates.Api.Common.Validation;

internal static class GuidValidator
{
    public static IDictionary<string, string[]>? ValidateGuid(string guid, string fieldName)
    {
        return !Guid.TryParse(guid, out var _)
            ? new Dictionary<string, string[]>
            {
                { "fieldName", [$"{fieldName} must be a valid GUID."] },
            }
            : null;
    }
}
