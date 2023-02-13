namespace LearnDotnetAuthentication.Data;

public class User
{
    public int Id{ get; set; }
    public string UserName { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
    public List<Character>? Characters { get; set; }
}