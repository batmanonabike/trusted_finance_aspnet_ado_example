using TrustedAbstractions;
using TrustedSqlDatabase;
using TrustedTests.Helpers;
using Xunit.Abstractions;

namespace TrustedTests.SqlLibraryTests
{
    public sealed class LibraryFactory : ILibraryFactory
    {
        private readonly LibrarySettings _settings;

        public LibraryFactory()
        {
            var config = new ConfigReader();
            _settings = new() { ConnectionString = config.LibraryConnectionString };
        }

        public ILibrary Create(TraceOutput output) => 
            new SelfCleaningLibrary(new Library(_settings), output);
    }

    [Collection(TestGroupNames.SqlLibrary)]
    public class BookTests(LibraryFactory libraryFactory, ITestOutputHelper output)
        : BookTester(libraryFactory, output), IClassFixture<LibraryFactory>
    {
    }
}
