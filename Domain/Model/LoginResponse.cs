namespace Domain.Model;

public class LoginResponse
{
    public LoginData Data { get; set; }
    public bool Success { get; set; }
    public string[] Errors { get; set; }
    public string Message { get; set; }
}
public class LoginData
{
    public Usuario Usuario { get; set; }
    public string Token { get; set; }
}
public class Usuario
{
    public string Username { get; set; }
    public string Password { get; set; }
}