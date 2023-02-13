using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace LearnDotnetAuthentication.Data;

public class AuthRepository : IAuthRepository
{
    private readonly DataContext _dataContext;
    private readonly IConfiguration _configuration;

    public AuthRepository(DataContext dataContext, IConfiguration configuration)
    {
        _dataContext = dataContext;
        _configuration = configuration;
    }
    
    public async Task<ServiceResponse<int>> Register(User user, string password)
    {
        var response = new ServiceResponse<int>();

        if (!await UserExists(user.UserName))
        {
            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();

            response.Data = user.Id;
        }
        else
        {
            response.Success = false;
            response.Message = "User already exists";
        }
        return response;
    }

    public async Task<ServiceResponse<string>> Login(string userName, string password)
    {
        var response = new ServiceResponse<string>();

        var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName.ToLower().Equals(userName.ToLower()));

        if (user is null)
        {
            response.Success = false;
            response.Message = "User not found.";
        }
        else if (VerifyPassword(password, user.PasswordHash, user.PasswordSalt) is false)
        {
            response.Success = false;
            response.Message = "Password not correct.";
        }
        else
        {
            //response.Data = user.Id.ToString();
            response.Data = CreateToken(user);
        }

        return response;
    }

    public async Task<bool> UserExists(string userName) =>
        await _dataContext.Users.AnyAsync(u => u.UserName.ToLower() == userName.ToLower());

    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512();

        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }

    private static bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt);

        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

        return computedHash.SequenceEqual(passwordHash);
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;

        if (appSettingsToken is null) throw new Exception("AppSettings Token is null");

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(appSettingsToken));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}