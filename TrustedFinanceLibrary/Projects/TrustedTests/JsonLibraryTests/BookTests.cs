using TrustedAbstractions;
using TrustedTests.Helpers;
using TrustedJsonDatabase;
using Xunit.Abstractions;

namespace TrustedTests.JsonLibraryTests
{
    public class LibraryFactory : ILibraryFactory
    {
        public ILibrary Create(TraceOutput output) => 
            new SelfCleaningLibrary(new Library(new LibrarySettings()), output);
    }

    [Collection(TestGroupNames.JsonLibrary)]
    public class BookTests(LibraryFactory libraryFactory, ITestOutputHelper output)
        : BookTester(libraryFactory, output), IClassFixture<LibraryFactory>
    {
    }
}
