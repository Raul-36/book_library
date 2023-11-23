using Azure.Core;
using GeneralClasses;
using Server.SQLRepositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server.RequestHandlers;

public class UserRequestHandler
{
    IUsersSQLRepository usersSQLRepository;

    public UserRequestHandler(IUsersSQLRepository usersSQLRepository)
    {
        this.usersSQLRepository = usersSQLRepository;       
    }
    public void ProcessTheRequest(HttpListenerContext context)
    {
        if (context.Request.RawUrl.Contains("users") == false)
            throw new Exception("unprocessed raw");
        if (context.Request.HttpMethod == HttpMethod.Get.Method)
        {
            Login(context);
        }
        else if (context.Request.HttpMethod == HttpMethod.Post.Method)
        {
            Register(context);
        }
        else if (context.Request.HttpMethod == HttpMethod.Delete.Method)
        {
            Delete(context);
        }
        else
        {
            using StreamWriter writer = new StreamWriter(context.Response.OutputStream);
            context.Response.StatusCode = 400;
            writer.WriteLine("incorrect request method");
        }
    }
    private void Login(HttpListenerContext context)
    {
        ObjectAndMessage<User> userOrMessage = new ObjectAndMessage<User>();

        context.Response.ContentType = "application/json";
        StreamWriter writer = new StreamWriter(context.Response.OutputStream);
       
        if (context.Request.ContentType != "application/json")
        {
            context.Response.StatusCode = 400;
            userOrMessage.Message = "incorrect content Type request";
        }
        else if (context.Request.InputStream == null)
        {
            context.Response.StatusCode = 400;
            userOrMessage.Message = "the request does not have a body";
        }
        else
        {
            StreamReader reader = new StreamReader(context.Request.InputStream);
            User user = JsonSerializer.Deserialize<User>(reader.ReadToEnd());
            if (user == null)
            {
                context.Response.StatusCode = 400;
                userOrMessage.Message = "null was sent instead of a value";
            }
            else
            {
                User userFromDb = usersSQLRepository.GetById(user.Id);
                if (userFromDb == null)
                {
                    context.Response.StatusCode = 404;
                    userOrMessage.Message = "User not found";

                }
                else if (userFromDb.Id == user.Id && userFromDb.Password == user.Password)
                {
                    context.Response.StatusCode = 200;
                    userOrMessage.TObject = userFromDb;
                }
                else
                {
                    context.Response.StatusCode = 403;
                    userOrMessage.Message = "incorrect ID or password";
                }
            }
            writer.WriteLine(JsonSerializer.Serialize(userOrMessage));
            writer.Dispose();
        }
    }


    private void Delete(HttpListenerContext context)
    {
        context.Response.ContentType = "text/plain";
        StreamWriter writer = new StreamWriter(context.Response.OutputStream);

        var id = context.Request.QueryString["id"];
        int idToInt;
        if (id == null || (int.TryParse(id, out idToInt)) == false){
            context.Response.StatusCode = 400;
            writer.WriteLine("id for deletion was not passed");
        }
        else if (usersSQLRepository.GetById(idToInt) == null)
        {
            context.Response.StatusCode = 404;
            writer.WriteLine("user not found");
        }
        else if (context.Request.ContentType != "application/json")
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
            else if (user.Id != idToInt) {
                context.Response.StatusCode = 401;
                writer.WriteLine("ID does not match sender ID");
            }
            else if (user.Password != (usersSQLRepository.GetById(idToInt)?.Password ?? string.Empty))
            {
                context.Response.StatusCode = 401;
                writer.WriteLine("the sent data does not match the user data");
            }
            else
            {
                usersSQLRepository.Delete(idToInt);
                context.Response.StatusCode = 200;
                writer.WriteLine("user deleted");
            }
        }

        writer.Dispose();
    }
    private void Register(HttpListenerContext context)
    {
        context.Response.ContentType = "text/plain";
        StreamWriter writer = new StreamWriter(context.Response.OutputStream); 
        if (context.Request.ContentType != "application/json")
        {
            context.Response.StatusCode = 400;
            writer.WriteLine("incorrect content Type request");
        }
        else
        {
            StreamReader reader = new StreamReader (context.Request.InputStream);
            User user = JsonSerializer.Deserialize<User>(reader.ReadToEnd());
            if (user == null)
            {
                context.Response.StatusCode = 400;
                writer.WriteLine("null was sent instead of a value");
            }
            else if((string.IsNullOrEmpty(user.Name) || string.IsNullOrWhiteSpace(user.Name))||
                (string.IsNullOrEmpty(user.Password) || string.IsNullOrWhiteSpace(user.Password)))
            {
                context.Response.StatusCode = 400;
                writer.WriteLine("required fields were either not initialized or initialized incorrectly");
            }
            else
            {
                usersSQLRepository.Create(user);
                context.Response.StatusCode =200;
                writer.WriteLine(user.Id);
            }
            writer.Dispose();
        }
    }
}
