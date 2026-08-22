using TrustedAbstractions;

namespace TrustedTests.Helpers
{
    public delegate void TraceOutput(string message);

    public interface ILibraryFactory
    {
        ILibrary Create(TraceOutput output);
    }
}
