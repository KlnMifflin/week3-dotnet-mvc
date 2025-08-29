using BookLibraryAPP.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookLibraryAPP.Controllers
{
    public class BooksController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BooksController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        //Show all books
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("BooksAPI");
            var books = await client.GetFromJsonAsync<List<Book>>("books");
            return View(books ?? new List<Book>()); //return view of list of books if one exists, otherwise create new list
        }

        //Show Create view
        public IActionResult Create() { return View(); }

        //Create new book
        [HttpPost]
        public async Task<IActionResult> Create(Book book)
        {
            if (!ModelState.IsValid) return View(book); //if required fields aren't filled, return to this view

            var client = _httpClientFactory.CreateClient("BooksAPI");
            var response = await client.PostAsJsonAsync("books", book);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Book creation failed.");
                return View(book);
            }

            return RedirectToAction("Index");
        }

        //Show Edit view
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("BooksAPI");
            var book = await client.GetFromJsonAsync<Book>($"books/{id}");

            if (book == null) return NotFound();

            return View(book);
        }

        //Update book
        [HttpPost]
        public async Task<IActionResult> Edit(Book book)
        {
            if (!ModelState.IsValid) return View(book);

            var client = _httpClientFactory.CreateClient("BooksAPI");
            var response = await client.PutAsJsonAsync($"books/{book.Id}", book);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to update book.");
                return View(book);
            }

            return RedirectToAction("Index");
        }

        //Delete book
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("BooksAPI");
            var response = await client.DeleteAsync($"books/{id}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to delete book.");
            }

            return RedirectToAction("Index");
        }
    }
}
