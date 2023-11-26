 using Server.Data;
using Server.SQLRepositories;
using Server.SQLRepositories.Base;
using System.ComponentModel;
using System.Net;
using SimpleInjector;
using Server.RequestHandlers;
using System.Xml.Linq;
using GeneralClasses;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

var container = ContainerSetup();

UsersRequestHandler usersRequestHandler = new UsersRequestHandler(container.GetInstance<IUsersSQLRepository>());
BooksRequestHandler booksRequestHandler = new BooksRequestHandler(container.GetInstance<IBooksSQLRepository>(), container.GetInstance<IUsersSQLRepository>());
container.GetInstance<LibraryDbContext>().Database.Migrate();
container.GetInstance<LibraryDbContext>().ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking; ;
HttpListener listener = new HttpListener();
const int port = 3636;
listener.Prefixes.Add($"http://localhost:{port}/");
listener.Start();

while ( true)
{
    HttpListenerContext context = listener.GetContext();
    var raw = context.Request.RawUrl;
    Console.WriteLine( raw );
    _ = Task.Run(() =>
    {
        bool containsUsers = raw.Contains("users");
        bool containsBooks = raw.Contains("books");
        if ((containsUsers && containsBooks) || (containsUsers == false && containsBooks == false))
        {
            using StreamWriter writer = new StreamWriter(context.Response.OutputStream);
            context.Response.StatusCode = 400;
            writer.WriteLine("incorrect request raw url");
        }
        else if (containsUsers)
        {
            usersRequestHandler.ProcessTheRequest(context);
        }
        else if (containsBooks)
        {
            booksRequestHandler.ProcessTheRequest(context);
        }


    });
}
    
SimpleInjector.Container ContainerSetup()
{
    var container = new SimpleInjector.Container();

    container.RegisterSingleton<LibraryDbContext>();
    container.RegisterSingleton<IUsersSQLRepository, UsersEFRepository>();
    container.RegisterSingleton<IBooksSQLRepository, BooksEFRepository>();
    container.Verify();
    return container;
}

