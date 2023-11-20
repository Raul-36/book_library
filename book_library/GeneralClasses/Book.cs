using System.Text.Json.Serialization;

namespace GeneralClasses;

public class Book
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public int? UserId { get; set; }

    [JsonIgnore]
    public User? User { get; set; }
}

