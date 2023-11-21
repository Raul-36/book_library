using Server.Data;
using System.Net;

LibraryDbContext libraryDbContext = new LibraryDbContext();
HttpListener listener = new HttpListener();
const int port = 3636;
listener.Prefixes.Add($"http://localhost:{port}/");
listener.Start();
while ( true)
{
    HttpListenerContext context = listener.GetContext();
    var re= context.Request.RawUrl;
  
}