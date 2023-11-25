using GeneralClasses;
using System.Text;
using System.Text.Json;

namespace Client
{
    public class BookService
    {
        private readonly HttpClient httpClient;
        private readonly string baseAddress;

        public BookService(HttpClient httpClient, string baseAddress)
        {
            this.httpClient = httpClient;
            this.baseAddress = baseAddress;
        }

        public async Task DisplayAllBooks(int userId)
        {
            var response = await httpClient.GetAsync($"{baseAddress}/books/all");

            if (response.IsSuccessStatusCode)
            {
                var allBooks = await response.Content.ReadAsStreamAsync();
                var books = JsonSerializer.Deserialize<List<Book>>(allBooks);

                Console.WriteLine("List of all books:");
                for (int i = 0; i < books.Count; i++)
                {
                    string status = "Available";

                    if (books[i].UserId == userId)
                    {
                        status = "Taken by you";
                    }

                    Console.WriteLine($"{i + 1}. ID: {books[i].Id}, Name: {books[i].Name}, Author: {books[i].Author}, Status: {status}");
                }

                Console.Write("Enter the number of the book you want to take (or 0 to cancel): ");
                int choice;
                if (int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= books.Count)
                {
                    await TakeABook(userId, books[choice - 1].Id, books[choice - 1].Name, books[choice - 1].Author);
                }
                else
                {
                    Console.WriteLine("Invalid choice or canceled.");
                }
            }
            else
            {
                Console.WriteLine($"Error getting a list of all books: {response.StatusCode}");
            }
        }

        public async Task TakeABook(int userId, int bookId, string bookName, string bookAuthor)
        {
            var takeBookData = new
            {
                UserId = userId,
                BookId = bookId,
                BookName = bookName,
                BookAuthor = bookAuthor,
            };

            var content = new StringContent(JsonSerializer.Serialize(takeBookData), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{baseAddress}/books/take", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Book {bookId} taken successfully.");
            }
            else
            {
                Console.WriteLine($"Error taking the book: {response.StatusCode}");
            }
        }

        public async Task DisplayMyBooks(int userId)
        {
            var response = await httpClient.GetAsync($"{baseAddress}/books/mybooks?userId={userId}");

            if (response.IsSuccessStatusCode)
            {
                var myBooks = await response.Content.ReadAsStreamAsync();
                var books = JsonSerializer.Deserialize<List<Book>>(myBooks);

                Console.WriteLine("List of your books:");
                foreach (var book in books)
                {
                    Console.WriteLine($"ID: {book.Id}, Name: {book.Name}, Author: {book.Author}");
                }
            }
            else
            {
                Console.WriteLine($"Error getting your books: {response.StatusCode}");
            }
        }
    }
}
