using System.Linq;
using System.Threading.Tasks;
using BookAPI.Controllers;
using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookAPI.Tests
{
    public class BookControllerTests
    {
        private BookContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<BookContext>()
                .UseInMemoryDatabase(databaseName: "BookDB_" + System.Guid.NewGuid().ToString())
                .Options;
            return new BookContext(options);
        }

        [Fact]
        public async Task GetBooks_ReturnsAllBooks()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Books.Add(new Book { Id = 1, Title = "Book One", Author = "Author A", Category = "Fiction", Language = "English" });
            context.Books.Add(new Book { Id = 2, Title = "Book Two", Author = "Author B", Category = "Non-Fiction", Language = "English" });
            await context.SaveChangesAsync();

            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<BookController>();
            var repository = new BookRepository(context); // Assuming BookRepository implements IBookRepository
            var controller = new BookController(repository, logger);

            // Act
            var result = await controller.GetBooks();
            var books = result.Value?.ToList() ?? new List<Book>();

            // Assert
            Assert.Equal(2, books.Count);
        }

        [Fact]
        public async Task GetBookById_BookExists_ReturnsBook()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var book = new Book { Id = 1, Title = "Existing Book", Author = "Author X", Category = "Fiction", Language = "English" };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<BookController>();
            var repository = new BookRepository(context); // Assuming BookRepository implements IBookRepository
            var controller = new BookController(repository, logger);

            // Act
            var result = await controller.GetBookById(1);

            // Assert
            Assert.IsType<Book>(result.Value);
            Assert.Equal("Existing Book", result.Value.Title);
        }

        [Fact]
        public async Task GetBookById_BookDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<BookController>();
            var repository = new BookRepository(context); // Assuming BookRepository implements IBookRepository
            var controller = new BookController(repository, logger);

            // Act
            var result = await controller.GetBookById(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetBooksByAuthor_BooksExist_ReturnsBooks()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Books.Add(new Book { Id = 1, Title = "Book One", Author = "Author A", Category = "Fiction", Language = "English" });
            context.Books.Add(new Book { Id = 2, Title = "Book Two", Author = "Author A", Category = "Mystery", Language = "English" });
            context.Books.Add(new Book { Id = 3, Title = "Book Three", Author = "Author B", Category = "Fiction", Language = "English" });
            await context.SaveChangesAsync();

            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<BookController>();
            var repository = new BookRepository(context); // Assuming BookRepository implements IBookRepository
            var controller = new BookController(repository, logger);

            var result = await controller.GetBooksByAuthor("Author A");
            var books = result.Value?.ToList() ?? new List<Book>();

            // Assert
            Assert.Equal(2, books.Count);
            Assert.All(books, b => Assert.Equal("Author A", b.Author));
        }

        [Fact]
        public async Task GetBooksByAuthor_NoBooksFound_ReturnsNotFound()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<BookController>();
            var repository = new BookRepository(context); // Assuming BookRepository implements IBookRepository
            var controller = new BookController(repository, logger);
            // Act
            var result = await controller.GetBooksByAuthor("NonExistent Author");

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

    }
}