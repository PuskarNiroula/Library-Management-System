using Library_Management_System.Data;
using Library_Management_System.DTOs.Book;
using Library_Management_System.Models;
using Library_Management_System.Services.Admin.Exception;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services.Admin.Book;

public class BookService(ApplicationDbContext dbContext) : IBookService
{
    private readonly ApplicationDbContext _context = dbContext;

    /// <summary>
    /// Create and persist a new Book entity from the provided DTO.
    /// </summary>
    /// <param name="createBookDto">DTO containing data for the new book: BookName, Author, CategoryId, Quantity, Price, Publisher, ImageUrl, PublishDate, and optional Isbn.</param>
    /// <returns>`true` if the book was saved to the database, `false` otherwise.</returns>
    public async Task<bool> CreateBookAsync(CreateBookDto createBookDto)
    {
        var book = new Models.Book
        {
            Author = createBookDto.Author,
            CategoryId = createBookDto.CategoryId,
            BookName = createBookDto.BookName,
            Quantity = createBookDto.Quantity,
            Price = (int)createBookDto.Price,
            Publisher = createBookDto.Publisher,
            ImageUrl = createBookDto.ImageUrl,
            PublicationDate = createBookDto.PublishDate
        };
        if (!string.IsNullOrEmpty(createBookDto.Author))
            book.Author = createBookDto.Author;
        if (!string.IsNullOrEmpty(createBookDto.Isbn))
            book.Isbn = createBookDto.Isbn;
        _context.Books.Add(book);
        return await _context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// Update an existing book record, optionally replace its stored image file, and persist the changes to the database.
    /// </summary>
    /// <param name="updateBookDto">Data transfer object containing the book identifier and fields to update.</param>
    /// <returns>`true` if at least one database row was affected, `false` otherwise (including when the specified book is not found).</returns>
    public async Task<bool> UpdateBookAsync(UpdateBookDto updateBookDto)
    {
        var book = await GetBookAsync(updateBookDto.BookId);
        //here it must have a try catch
        if (updateBookDto.ImageUrl != null && !string.IsNullOrEmpty(book.ImageUrl))
        {
            var oldImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", book.ImageUrl.TrimStart('/'));
            if (File.Exists(oldImage))
                File.Delete(oldImage);
        }

        book.Author = updateBookDto.Author;
        book.BookId = updateBookDto.BookId;
        book.CategoryId = updateBookDto.CategoryId;
        book.BookName = updateBookDto.BookName;
        book.Quantity = updateBookDto.Quantity;
        book.Price = (int)updateBookDto.Price;
        if (updateBookDto.ImageUrl != null)
            book.ImageUrl = updateBookDto.ImageUrl;
        book.Publisher = updateBookDto.Publisher;
        if (!string.IsNullOrEmpty(updateBookDto.Author))
            book.Author = updateBookDto.Author;
        if (!string.IsNullOrEmpty(updateBookDto.Isbn))
            book.Isbn = updateBookDto.Isbn;
        book.PublicationDate = updateBookDto.PublishDate;
        _context.Books.Update(book);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return false;
        _context.Books.Remove(book);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> SaleOrRentBook(int id,string type)
    {
        var book = await GetBookByIdAsync(id);
        if (book.Quantity > 0)
        {
            book.Quantity--;
            _context.Books.Update(book);
            return await _context.SaveChangesAsync() > 0;
        }
        return false;
    }

    public async Task<Models.Book> GetBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        return book ?? throw new BookNotFoundException(id);
    }

    /// <summary>
    /// Retrieve the four most recently added books ordered from newest to oldest.
    /// </summary>
    /// <returns>A list containing up to four Book entities, ordered by descending BookId (newest first).</returns>
    public async Task<List<Models.Book>> GetNewBooks()
    {
        var newBooks = await _context.Books.OrderByDescending(b => b.BookId).Take(4).ToListAsync();
        return newBooks;
    }

    /// <summary>
    /// Searches books by matching the provided text against BookName, Author, Isbn, CategoryName, or Publisher and returns the specified page of results.
    /// </summary>
    /// <param name="searchString">Text to match against BookName, Author, Isbn, CategoryName, or Publisher.</param>
    /// <param name="page">1-based page index to return.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <returns>A PaginatedBook&lt;Models.Book&gt; containing the matching books for the requested page and pagination metadata.</returns>
    public async Task<PaginatedBook<Models.Book>> GetSearchedBook(string searchString, int page = 1, int pageSize = 6)
    {
        var totalBooks = await _context.Books.Where(b => b.BookName.Contains(searchString)
                                                         || b.Author.Contains(searchString)
                                                         || b.Isbn.Contains(searchString)
                                                         || (b.Category != null &&
                                                             b.Category.CategoryName.Contains(searchString))
                                                         || b.Publisher.Contains(searchString)).CountAsync();

        var books = await _context.Books.Where(b => b.BookName.Contains(searchString)
                                                    || b.Author.Contains(searchString)
                                                    || b.Isbn.Contains(searchString)
                                                    || b.Category.CategoryName.Contains(searchString)
                                                    || b.Publisher.Contains(searchString))
            .OrderBy(b => b.BookName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return MakePaginatedModel(books, page, pageSize, totalBooks);
    }


    /// <summary>
    /// Retrieve a page of books ordered by book name and packaged with pagination metadata.
    /// </summary>
    /// <param name="page">1-based page index to retrieve.</param>
    /// <param name="pageSize">Number of books per page.</param>
    /// <returns>A PaginatedBook&lt;Models.Book&gt; containing the page of books, the current page, page size, total page count, and total item count.</returns>
    public async Task<PaginatedBook<Models.Book>> GetPaginatedBooks(int page = 1, int pageSize = 6)
    {
        var skip = (page - 1) * pageSize;
        var totalBooks = await _context.Books.CountAsync();

        var books = await _context.Books
            .OrderBy(b => b.BookName)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
        return MakePaginatedModel(books, page, pageSize, totalBooks);
    }

    public async Task<Models.Book> GetBookByIdAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        return book ?? throw new BookNotFoundException(id);
    }

    private PaginatedBook<Models.Book> MakePaginatedModel(List<Models.Book> books, int page, int pageSize,
        int totalBooks)
    {
        var totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);

        return new PaginatedBook<Models.Book>()
        {
            Items = books,
            CurrentPage = page,
            TotalPages = totalPages,
            PageSize = pageSize,
            TotalCount = totalBooks
        };
    }
}