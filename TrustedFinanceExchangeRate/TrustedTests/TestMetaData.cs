namespace TrustedTests
{
    internal static class TestGroupNames
    {
        public const string WebApi = "WebApi";
    }

    [CollectionDefinition(TestGroupNames.WebApi, DisableParallelization = true)]
    public sealed class WebApiTestGroup
    {
    }
}
