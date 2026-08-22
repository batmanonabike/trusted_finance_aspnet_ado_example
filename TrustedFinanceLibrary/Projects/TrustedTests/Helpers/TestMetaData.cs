namespace TrustedTests.Helpers
{
    // Preparing to protect against concurrency if more tests run against the same store.
    internal static class TestGroupNames
    {
	    public const string WebApp = "WebApp";
        public const string SqlLibrary = "SqlLibrary";
        public const string JsonLibrary = "JsonLibrary";
    }

    [CollectionDefinition(TestGroupNames.SqlLibrary, DisableParallelization = true)]
    public sealed class SqlLibraryTestGroup
    {
    }

    [CollectionDefinition(TestGroupNames.JsonLibrary, DisableParallelization = true)]
    public sealed class JsonLibraryTestGroup
    {
    }

    // Also touches the shared JSON store, so keep it out of parallel runs too.
    [CollectionDefinition(TestGroupNames.WebApp, DisableParallelization = true)]
    public sealed class WebAppTestGroup
    {
    }
}
