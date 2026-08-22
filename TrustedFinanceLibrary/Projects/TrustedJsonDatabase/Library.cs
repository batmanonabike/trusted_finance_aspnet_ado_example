using TrustedAbstractions;
using TrustedJsonDatabase.Helpers;

namespace TrustedJsonDatabase
{
    public sealed class LibrarySettings
    {
        public string JsonLibrary { get; }
        public string AppDataDirectory { get; }

        public LibrarySettings()
        {
            AppDataDirectory = FileSystemTools.GetAppDataDirectory();
            JsonLibrary = FileSystemTools.GetOrCreateJsonLibrary(AppDataDirectory);
        }
    }

    public class Library(LibrarySettings settings) : IDisposable, ILibrary
    {
        private bool _disposed;
        private readonly IBookRepository _bookRepository = new BookRepository(settings);

        public bool IsOpen() => true;
        public IBookRepository Books => _bookRepository;

        public void Dispose()
        {
            if (_disposed) return;
            //if (!string.IsNullOrWhiteSpace(settings.JsonLibrary))
            //    File.Delete(settings.JsonLibrary);

            _disposed = true;
        }
    }
}
