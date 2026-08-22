using TrustedAbstractions;

namespace TrustedTests.Helpers
{
    internal sealed class SelfCleaningLibrary : IDisposable, ILibrary
    {
        private bool _disposed;
        private readonly ILibrary _library;
        private readonly TraceOutput _output;
        private readonly IdTracker _bookIdTracker;
        private readonly IBookRepository _bookRepository;
        private readonly IBookRepository _trackingBookRepository;

        public SelfCleaningLibrary(ILibrary library, TraceOutput output)
        {
            _output = output;
            _library = library;
            _bookRepository = _library.Books;

            _bookIdTracker = new IdTracker();
            _trackingBookRepository = new TrackingBookRepository(_bookRepository, _bookIdTracker);
        }

        public bool IsOpen() => _library.IsOpen();
        public IBookRepository Books => _trackingBookRepository;

        private void DeleteBooks()
        {
            int deleteCount = 0;
            var bookIds = _bookIdTracker.Ids;

            foreach (var id in bookIds)
            {
                try
                {
                    if (_bookRepository.Delete(id))
                        deleteCount++;
                }
                catch (Exception ex)
                {
                    _output($"Failed to delete test book {id}: {ex.Message}");
                }
            }

            _output($"Deleted: {deleteCount}/{bookIds.Count()} book(s)");
        }

        public void Dispose()
        {
            if (_disposed) return;

            DeleteBooks();
            _library.Dispose();
            _disposed = true;
        }
    }
}
