using Kolokwium_APBD.Models;

namespace Kolokwium_APBD.Services;

public interface IBooksAuthorsService
{
    public Task<BooksAuthors> GetInfo (int id);
    public Task<BooksAuthors> CreateNewBookAndAuthors (BooksAuthorsNoId bookAuthorsNoId);
}