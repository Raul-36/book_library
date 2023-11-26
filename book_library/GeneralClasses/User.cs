using System.Text.Json.Serialization;

namespace GeneralClasses;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
}
