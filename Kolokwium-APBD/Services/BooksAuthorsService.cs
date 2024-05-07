using System.Data.SqlClient;
using Kolokwium_APBD.Models;

namespace Kolokwium_APBD.Services;

public class BooksAuthorsService : IBooksAuthorsService
{
    private async Task<SqlConnection> GetConnection()
    {
        var connection = new SqlConnection("Server=localhost;Database=master;User Id=SA;Password=yourStrong(!)Password;");
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }
        return connection;
    }
    
    public async Task<BooksAuthors> GetInfo(int id)
    {
        await using var connection = await GetConnection();
        await using var command =
            new SqlCommand(
                "select books.PK,title,FK_author,first_name,last_name from books, authors, books_authors where books.PK = books_authors.FK_book and authors.PK = books_authors.FK_author and books.PK = @IdBook",
                connection);
        command.Parameters.AddWithValue("@IdBook", id);
        
        await using var reader = await command.ExecuteReaderAsync();
        if (!reader.HasRows)
        {
            return null;
        }
        var booksAuthors = new BooksAuthors() {
            Authors = new List<Authors>()
        };
        while (await reader.ReadAsync())
        {
            booksAuthors.IdBook = reader.GetInt32(0);
            booksAuthors.Title = reader.GetString(1);
            booksAuthors.Authors.Add(new Authors(reader.GetString(3), reader.GetString(4)));
        }
        return booksAuthors;
    }
public async Task<BooksAuthors> CreateNewBookAndAuthors(BooksAuthorsNoId booksAuthorsNoId)
{
    var connection = await GetConnection();
    await using var transaction = (SqlTransaction) await connection.BeginTransactionAsync();

    try
    {
        await using var command = new SqlCommand("Insert into Books (Title) values (@Title)", connection, transaction);
        command.Parameters.AddWithValue("@Title", booksAuthorsNoId.Title);
        await command.ExecuteNonQueryAsync();

        await using (var command2 = new SqlCommand("select max(PK) from books", connection, transaction))
        {
            await using var reader = await command2.ExecuteReaderAsync();
            if (!reader.HasRows)
            {
                return null;
            }
            await reader.ReadAsync();
            var idBook = reader.GetInt32(0);
            await reader.CloseAsync();

            foreach (var author in booksAuthorsNoId.Authors)
            {
                await using var command3 = new SqlCommand("INSERT INTO Authors (first_name, last_name) VALUES (@FirstName, @LastName)", connection, transaction);
                command3.Parameters.AddWithValue("@FirstName", author.Name);
                command3.Parameters.AddWithValue("@LastName", author.Surname);
                await command3.ExecuteNonQueryAsync();

                await using var command4 = new SqlCommand("select max(PK) from Authors", connection, transaction);
                await using var reader2 = await command4.ExecuteReaderAsync();
                if (!reader2.HasRows)
                {
                    return null;
                }
                await reader2.ReadAsync();
                var IdAuthor = reader2.GetInt32(0);
                await reader2.CloseAsync();

                await using var command5 = new SqlCommand("Insert into Books_Authors (FK_Book, FK_Author) values (@FK_Book, @FK_Author)", connection, transaction);
                command5.Parameters.AddWithValue("@FK_Book", idBook);
                command5.Parameters.AddWithValue("@FK_Author", IdAuthor);
                await command5.ExecuteNonQueryAsync();
            }
            var result = new BooksAuthors()
            {
                IdBook = idBook,
                Title = booksAuthorsNoId.Title,
                Authors = booksAuthorsNoId.Authors
            };

            await transaction.CommitAsync();

            return result;
        }
    }
    catch (Exception)
    {
        await transaction.RollbackAsync();
        throw;
    }
}
}