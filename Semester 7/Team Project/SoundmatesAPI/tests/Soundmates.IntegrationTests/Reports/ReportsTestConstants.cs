namespace Soundmates.IntegrationTests.Reports;

/// <summary>
/// Route and reusable domain constants specific to the Reports domain tests
/// (3.37 - 3.38 in tests-plan.md). Do NOT add entries to the shared <c>TestConstants</c>.
/// </summary>
internal static class ReportsTestConstants
{
    public const string ReportUserRoute = "/reports";

    /// <summary>Builds POST /reports/{userId}/block.</summary>
    public static string BlockUserRoute(object userId) => $"/reports/{userId}/block";

    // Validator boundaries — mirror ReportUserValidator.
    public const int ReasonMaxLength = 200;
    public const int DescriptionMaxLength = 1000;

    // Strings that sit at the boundary (exactly at max length — must pass).
    public static string ReasonAtMaxLength() => new('R', ReasonMaxLength);
    public static string DescriptionAtMaxLength() => new('D', DescriptionMaxLength);

    // Strings that exceed the boundary (one char over — must fail).
    public static string ReasonOverMaxLength() => new('R', ReasonMaxLength + 1);
    public static string DescriptionOverMaxLength() => new('D', DescriptionMaxLength + 1);

    // Default reason/description used in happy-path tests.
    public const string DefaultReason = "Inappropriate behaviour";
    public const string DefaultDescription = "This user sent me offensive messages.";
}
