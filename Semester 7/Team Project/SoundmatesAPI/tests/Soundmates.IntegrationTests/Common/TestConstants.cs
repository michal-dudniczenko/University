namespace Soundmates.IntegrationTests.Common;

internal static class TestConstants
{
    public const string DbDockerImageTag = "mcr.microsoft.com/mssql/server:2025-CU5-ubuntu-24.04";
    public const string TestDatabaseName = "SoundmatesTests";
    public const string IntegrationTestsCollectionName = "IntegrationTests";

    public static readonly string[] DictionaryTableNames =
    [
        "Countries",
        "Cities",
        "Genders",
        "BandRoles",
        "TagCategories",
        "Tags"
    ];
}
