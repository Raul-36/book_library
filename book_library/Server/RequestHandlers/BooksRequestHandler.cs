using GeneralClasses;
using Microsoft.EntityFrameworkCore;
using Server.SQLRepositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server.RequestHandlers;
public class BooksRequestHandler
{
    IBooksSQLRepository books;
    IUsersSQLRepository users;
    public BooksRequestHandler(IBooksSQLRepository books, IUsersSQLRepository users)
    {
        this.books = books;
        this.users = users;
    }
    public void ProcessTheRequest(HttpListenerContext context)
    {
        if (context.Request.RawUrl.Contains("books") == false)
        {
            using StreamWriter writer = new StreamWriter(context.Response.OutputStream);
            context.Response.StatusCode = 500;
            writer.WriteLine("Server ERROR");

            throw new Exception("unprocessed raw");
        }
        if (context.Request.HttpMethod == HttpMethod.Get.Method)
        {
            GetBooks(context);
        }
        else if (context.Request.HttpMethod == HttpMethod.Put.Method)
        {
            if (context.Request.RawUrl.Contains("borrow") == true)
                Borrow(context);
            else if (context.Request.RawUrl.Contains("return") == true)
                Return(context);
            else
            {
                using StreamWriter writer = new StreamWriter(context.Response.OutputStream);
                context.Response.StatusCode = 400;
                writer.WriteLine("incorrect request raw url");
            }
        }
        else
        {
            using StreamWriter writer = new StreamWriter(context.Response.OutputStream);
            context.Response.StatusCode = 400;
            writer.WriteLine("incorrect request method");
        }
    }
   
    private void GetBooks(HttpListenerContext context)
    {
        context.Response.ContentType = "application/json";
        StreamWriter writer = new StreamWriter(context.Response.OutputStream);
            writer.WriteLine(JsonSerializer.Serialize(books.GetAll()));
        
        writer.Dispose();
    }
    private void Return(HttpListenerContext context)
    {
        context.Response.ContentType = "text/plain";
        StreamWriter writer = new StreamWriter(context.Response.OutputStream);

        var id = context.Request.QueryString["id"];
        int idToInt;
        if (id == null || (int.TryParse(id, out idToInt)) == false)
        {
            context.Response.StatusCode = 400;
            writer.WriteLine("id for borrow was not passed");
        }
        else if (books.GetById(idToInt) == null)
        {
            context.Response.StatusCode = 404;
            writer.WriteLine("book not found");
        }
        else if (context.Request.ContentType.Contains("application/json") == false)
        {
            context.Response.StatusCode = 400;
            writer.WriteLine("incorrect content Type request");
        }
        else if (context.Request.InputStream == null)
        {
            context.Response.StatusCode = 400;
            writer.WriteLine("the request does not have a body");
        }
        else
        {
            StreamReader reader = new StreamReader(context.Request.InputStream);
            User user = JsonSerializer.Deserialize<User>(reader.ReadToEnd());
            if (user == null)
            {
                context.Response.StatusCode = 400;
                writer.WriteLine("null was sent instead of a value");
            }
            else if (user.Password != (users.GetById(idToInt)?.Password ?? string.Empty))
            {
                context.Response.StatusCode = 401;
                writer.WriteLine("the sent data does not match the user data");
            }
            else
            {
                Book book = books.GetById(idToInt);
                if (book.UserId != user.Id)
                {
                    context.Response.StatusCode = 403;
                    writer.WriteLine("The book can only be returned by the person who borrowed it.");
                }
                else
                {
                    book.UserId = null;
                    user.BookId = null;
                    books.UpdateUserId(book.Id,book);
                    users.UpdateBookId(user.Id, user);

                    context.Response.StatusCode = 200;
                    writer.WriteLine("book returned");
                }
            }
        }
        writer.Dispose();
    }

 
    private void Borrow(HttpListenerContext context)
    {
        context.Response.ContentType = "text/plain";
        StreamWriter writer = new StreamWriter(context.Response.OutputStream);

        var id = context.Request.QueryString["id"];
        int idToInt;
        if (id == null || (int.TryParse(id, out idToInt)) == false)
        {
            context.Response.StatusCode = 400;
            writer.WriteLine("id for borrow was not passed");
        }
        else if (books.GetById(idToInt) == null)
        {
            context.Response.StatusCode = 404;
            writer.WriteLine("book not found");
        }
        else if (context.Request.ContentType.Contains("application/json") == false)
        {
            context.Response.StatusCode = 400;
            writer.WriteLine("incorrect content Type request");
        }
        else if (context.Request.InputStream == null)
        {
            context.Response.StatusCode = 400;
            writer.WriteLine("the request does not have a body");
        }
        else
        {
            StreamReader reader = new StreamReader(context.Request.InputStream);
            User user = JsonSerializer.Deserialize<User>(reader.ReadToEnd());
            if (user == null)
            {
                context.Response.StatusCode = 400;
                writer.WriteLine("null was sent instead of a value");
            }
            else if (user.Password != (users.GetById(user.Id)?.Password ?? string.Empty))
            {
                context.Response.StatusCode = 401;
                writer.WriteLine("the sent data does not match the user data");
            }
            else
            {
                Book book = books.GetById(idToInt);
                if (book.UserId != null)
                {
                    context.Response.StatusCode = 409;
                    writer.WriteLine("book is busy");
                }
                else if (user.BookId != null)
                {
                    context.Response.StatusCode = 409;
                    writer.WriteLine("you cannot take out a new book until you return the one you are reading now");
                }
                else
                {
                    book.UserId = user.Id;
                    user.BookId = book.Id;
                    books.UpdateUserId(book.Id, book);
                    users.UpdateBookId(user.Id, user);

                    context.Response.StatusCode = 200;
                    writer.WriteLine("book borrowed");
                }
            }
        }
        writer.Dispose();
    }
}
