using Microsoft.Data.SqlClient;
using TrustedAbstractions;

namespace TrustedSqlDatabase
{
    public sealed class LibrarySettings
    {
        public required string ConnectionString { get; init;  }
    }

    public sealed class Library : IDisposable, ILibrary
    {
        private bool _disposed;
        private readonly SqlConnection _connection;
        private readonly IBookRepository _bookRepository;

        public Library(LibrarySettings settings)
        {
            _connection = Connect(settings.ConnectionString);
            _bookRepository = new BookRepository(_connection);
        }

        public IBookRepository Books => _bookRepository;
        public bool IsOpen() => _connection.State == System.Data.ConnectionState.Open;

        private static SqlConnection Connect(string connectionString)
        {
            var connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                return connection;
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _connection.Dispose();
            _disposed = true;
        }
    }
}
