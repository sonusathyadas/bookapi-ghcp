using Microsoft.AspNetCore.Mvc;
using BookAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookAPI.Repositories;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Secure all actions in the controller
    public class BookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<BookController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookController"/> class.
        /// </summary>
        /// <param name="bookRepository">The book repository.</param>
        /// <param name="logger">The logger instance.</param>
        public BookController(IBookRepository bookRepository, ILogger<BookController> logger)
        {
            _bookRepository = bookRepository;
            _logger = logger;
        }

        /// <summary>
        /// Gets the list of all books.
        /// </summary>
        /// <returns>A list of books.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            _logger.LogInformation("Fetching all books.");
            try
            {
                var books = await _bookRepository.GetBooks();
                _logger.LogInformation("Successfully fetched all books.");
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the books.");
                return StatusCode(500, "An error occurred while retrieving the books. " + ex.Message);
            }
        }

        /// <summary>
        /// Gets a book by its ID.
        /// </summary>
        /// <param name="id">The ID of the book.</param>
        /// <returns>The book with the specified ID.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBookById(int id)
        {
            _logger.LogInformation("Fetching book with ID {Id}.", id);
            try
            {
                var book = await _bookRepository.GetBookById(id);

                if (book == null)
                {
                    _logger.LogWarning("Book with ID {Id} not found.", id);
                    return NotFound();
                }

                _logger.LogInformation("Successfully fetched book with ID {Id}.", id);
                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the book with ID {Id}.", id);
                return StatusCode(500, "An error occurred while retrieving the book. " + ex.Message);
            }
        }

        /// <summary>
        /// Gets books by the author's name.
        /// </summary>
        /// <param name="authorName">The name of the author.</param>
        /// <returns>A list of books by the specified author.</returns>
        [HttpGet("author/{authorName}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByAuthor(string authorName)
        {
            _logger.LogInformation("Fetching books by author {AuthorName}.", authorName);
            try
            {
                var books = await _bookRepository.GetBooksByAuthor(authorName);

                if (books == null)
                {
                    _logger.LogWarning("No books found for author {AuthorName}.", authorName);
                    return NotFound();
                }

                _logger.LogInformation("Successfully fetched books by author {AuthorName}.", authorName);
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving books by author {AuthorName}.", authorName);
                return StatusCode(500, "An error occurred while retrieving books by the author. " + ex.Message);
            }
        }

        /// <summary>
        /// Gets books by the category.
        /// </summary>
        /// <param name="category">The category of the books.</param>
        /// <returns>A list of books in the specified category.</returns>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByCategory(string category)
        {
            _logger.LogInformation("Fetching books in category {Category}.", category);
            try
            {
                var books = await _bookRepository.GetBooksByCategory(category);

                if (books == null || books.Count() == 0)
                {
                    _logger.LogWarning("No books found in category {Category}.", category);
                    return NotFound();
                }

                _logger.LogInformation("Successfully fetched books in category {Category}.", category);
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving books in category {Category}.", category);
                return StatusCode(500, "An error occurred while retrieving books by category. " + ex.Message);
            }
        }

        /// <summary>
        /// Creates a new book.
        /// </summary>
        /// <param name="book">The book to create.</param>
        /// <returns>The created book.</returns>
        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook([FromBody] Book book)
        {
            _logger.LogInformation("Creating a new book.");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for book creation.");
                return BadRequest(ModelState);
            }

            try
            {
                await _bookRepository.CreateBook(book);
                _logger.LogInformation("Successfully created a new book with ID {Id}.", book.Id);
                return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the book.");
                return StatusCode(500, "An error occurred while creating the book. " + ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing book.
        /// </summary>
        /// <param name="id">The ID of the book to update.</param>
        /// <param name="book">The updated book.</param>
        /// <returns>No content if the update is successful.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book book)
        {
            _logger.LogInformation("Updating book with ID {Id}.", id);
            if (id != book.Id)
            {
                _logger.LogWarning("The ID in the URL does not match the ID in the book object.");
                return BadRequest("The ID in the URL does not match the ID in the book object.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for book update.");
                return BadRequest(ModelState);
            }

            try
            {
                var existingBook = await _bookRepository.GetBookById(id);
                if (existingBook == null)
                {
                    _logger.LogWarning("Book with ID {Id} not found.", id);
                    return NotFound("The book with the specified ID does not exist.");
                }

                await _bookRepository.UpdateBook(book);
                _logger.LogInformation("Successfully updated book with ID {Id}.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the book with ID {Id}.", id);
                return StatusCode(500, "An error occurred while updating the book. " + ex.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a book by its ID.
        /// </summary>
        /// <param name="id">The ID of the book to delete.</param>
        /// <returns>No content if the deletion is successful.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            _logger.LogInformation("Deleting book with ID {Id}.", id);
            try
            {
                var book = await _bookRepository.GetBookById(id);
                if (book == null)
                {
                    _logger.LogWarning("Book with ID {Id} not found.", id);
                    return NotFound();
                }

                await _bookRepository.DeleteBook(id);
                _logger.LogInformation("Successfully deleted book with ID {Id}.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the book with ID {Id}.", id);
                return StatusCode(500, "An error occurred while deleting the book. " + ex.Message);
            }
        }
    }
}