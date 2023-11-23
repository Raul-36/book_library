 using Server.Data;
using Server.SQLRepositories;
using Server.SQLRepositories.Base;
using System.ComponentModel;
using System.Net;
using SimpleInjector;
using Server.RequestHandlers;

var container = new SimpleInjector.Container();

container.RegisterSingleton<LibraryDbContext>();
container.RegisterSingleton<IUsersSQLRepository, UsersEFRepository>();
container.RegisterSingleton<IBooksSQLRepository, BooksEFRepository>();

container.Verify();

UserRequestHandler userRequestHandler = new UserRequestHandler(container.GetInstance<IUsersSQLRepository>());

HttpListener listener = new HttpListener();
const int port = 3636;
listener.Prefixes.Add($"http://localhost:{port}/");
listener.Start();

while ( true)
{
    HttpListenerContext context = listener.GetContext();
    var raw = context.Request.RawUrl;
    if (raw.Contains("users"))
    {
        userRequestHandler.ProcessTheRequest(context);
    }
  
}