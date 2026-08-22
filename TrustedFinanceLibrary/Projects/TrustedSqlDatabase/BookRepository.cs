using System.Data;
using Microsoft.Data.SqlClient;
using TrustedTools;
using TrustedAbstractions;

namespace TrustedSqlDatabase
{
    public class BookRepository(SqlConnection connection) : IBookRepository
    {
        public Book Add(Book book)
        {
            using var command = new SqlCommand(@"
                INSERT INTO dbo.Books (Title, Author, PublishDate, Genre, Price)
                OUTPUT INSERTED.BookId
                VALUES (@Title, @Author, @PublishDate, @Genre, @Price)",
                connection);
            PrepareUpsert(command.Parameters, book);

            int bookId = (int)command.ExecuteScalar();
            return book.Clone(bookId);
        }

        public Book? Get(int id)
        {
            using var command = new SqlCommand($@"
                SELECT {SelectFieldsForReader} FROM dbo.Books
                WHERE BookId = @BookId",
                connection);
            command.Parameters.Add("@BookId", SqlDbType.Int).Value = id;

            using var reader = command.ExecuteReader();
            return reader.Read() ? CreateFromReader(reader) : null;
        }

        public List<Book> ReadAll()
        {
            using var command = new SqlCommand(
                $"SELECT {SelectFieldsForReader} FROM dbo.Books ORDER BY BookId",
                connection);

            using var reader = command.ExecuteReader();
            var result = new List<Book>();
            while (reader.Read())
                result.Add(CreateFromReader(reader));

            return result;
        }

        public int Count()
        {
            using var command = new SqlCommand("SELECT COUNT(*) FROM dbo.Books", connection);
            return (int)command.ExecuteScalar();
        }

        public bool Exists(int id)
        {
            using var command = new SqlCommand(
                "SELECT 1 FROM dbo.Books WHERE BookId = @BookId",
                connection);
            command.Parameters.Add("@BookId", SqlDbType.Int).Value = id;

            return command.ExecuteScalar() != null;
        }

        public bool Update(Book book)
        {
            using var command = new SqlCommand(@"
                UPDATE dbo.Books SET 
                    Title=@Title, Author=@Author, PublishDate=@PublishDate, Genre=@Genre, Price=@Price
                WHERE BookId = @BookId",
                connection);
            PrepareUpsert(command.Parameters, book);
            command.Parameters.Add("@BookId", SqlDbType.Int).Value = book.BookId;

            return command.ExecuteNonQuery() == 1;
        }

        public bool Delete(int id)
        {
            using var command = new SqlCommand(
                "DELETE FROM dbo.Books WHERE BookId = @BookId",
                connection);
            command.Parameters.Add("@BookId", SqlDbType.Int).Value = id;

            return command.ExecuteNonQuery() == 1;
        }

        private const int GenreLength = 50;
        private const int TitleLength = 400;
        private const int AuthorLength = 200;

        private static void PrepareUpsert(SqlParameterCollection parameters, Book book)
        {
            parameters.Add("@Price", SqlDbType.Decimal).Value = book.Price;
            parameters.Add("@PublishDate", SqlDbType.Date).Value = book.PublishDate;
            parameters.Add("@Genre", SqlDbType.NVarChar, GenreLength).Value = book.Genre;
            parameters.Add("@Title", SqlDbType.NVarChar, TitleLength).Value = book.Title;
            parameters.Add("@Author", SqlDbType.NVarChar, AuthorLength).Value = book.Author;
        }

        private const string SelectFieldsForReader = "BookId, Title, Author, PublishDate, Genre, Price";
        private static Book CreateFromReader(SqlDataReader reader)
        {
            return new Book
            {
                BookId = reader.GetInt32(0),
                Title = reader.GetString(1),
                Author = reader.GetString(2),
                PublishDate = reader.GetFieldValue<DateOnly>(3),
                Genre = reader.GetString(4),
                Price = reader.GetDecimal(5)
            };
        }
    }
}
