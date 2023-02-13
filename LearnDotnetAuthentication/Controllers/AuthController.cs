using Microsoft.AspNetCore.Mvc;

namespace LearnDotnetAuthentication.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;

    public AuthController(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    [HttpPost("Register")]
    public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto user)
    {
        var response = await _authRepository.Register(
            new User { UserName = user.UserName }, user.Password);

        if (!response.Success) return BadRequest(response);
        else return Ok(response);
    }
    
    [HttpPost("Login")]
    public async Task<ActionResult<ServiceResponse<string>>> Login(UserLoginDto user)
    {
        var response = await _authRepository.Login(user.UserName, user.Password);

        if (!response.Success) return BadRequest(response);
        else return Ok(response);
    }
}