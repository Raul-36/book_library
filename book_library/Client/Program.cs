using Client;
using GeneralClasses;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

class Program
{
    const string baseAddress = "http://localhost:3636";
    static HttpClient httpClient = new HttpClient();

    static async Task Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("1. Registration");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await Register();
                    break;
                case "2":
                    await Login();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Wrong choice. Try again");
                    break;
            }
        }
    }

    static async Task Register()
    {
        Console.Write("Enter name: ");
        string name = Console.ReadLine();

        Console.Write("Enter password: ");
        string password = Console.ReadLine();

        var newUser = new User
        {
            Name = name,
            Password = password,
        };

        var jsonUser = JsonSerializer.Serialize(newUser);
        var content = new StringContent(jsonUser, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{baseAddress}/users/registration", content);
        var responseTxt = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("User successfully registered");
            Console.WriteLine($"Your ID for login: {responseTxt}");
        }
        else
        {
            Console.WriteLine($"Content: {responseTxt}");
            Console.WriteLine($"Error during registration: ");
        }
    }

    static async Task Login()
    {
        Console.Write("Enter ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID. Try again.");
            return;
        }

        Console.Write("Enter password: ");
        string password = Console.ReadLine();

        var loginUser = new User
        {

            Id = id,
            Password = password,
        };

        var jsonUser = JsonSerializer.Serialize(loginUser);
        var content = new StringContent(jsonUser, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{baseAddress}/users/login", content);


        var responseContent = await response.Content.ReadAsStringAsync();
        var userOrMessage = JsonSerializer.Deserialize<ObjectAndMessage<User>>(responseContent);

        if (response.IsSuccessStatusCode)
        {
            if (userOrMessage != null && userOrMessage.TObject != null)
            {
                Console.WriteLine($"Login successful. User ID: {userOrMessage.TObject.Id}");
                await ShowMenu(userOrMessage.TObject.Id, userOrMessage.TObject.Name);
            }
            else
            {
                Console.WriteLine($"Login error: {userOrMessage?.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Login error: {userOrMessage?.Message}");
        }
    }

    static async Task ShowMenu(int Id, string name)
    {
        var bookService = new BookService(httpClient, baseAddress);

        while (true)
        {
            Console.WriteLine("1. Books");
            Console.WriteLine("2. My book");
            Console.WriteLine("3. Delete account");
            Console.WriteLine("4. Exit");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await bookService.DisplayAllBooks(Id);
                    break;
                case "2":
                    await bookService.DisplayMyBooks(Id);
                    break;
                case "3":
                    await DeleteAccount(name);
                    break;
                case "4":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Wrong choice. try again");
                    break;
            }
        }
    }

    static async Task DeleteAccount(string name)
    {
        var response = await httpClient.DeleteAsync($"{baseAddress}/delete/{name}");

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"User {name} account deleted");
            Environment.Exit(0);
        }
        else
        {
            Console.WriteLine($"Error deleting account: {response.StatusCode}");
        }
    }
}