
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnDotnetAuthentication.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CharacterController: ControllerBase
{
    private readonly ICharacterService _characterService;

    public CharacterController(ICharacterService characterService)
    {
        _characterService = characterService;
    }
    
    //With this tag access to this method is allowed, it means it is not necessary to get jwt token
    //[AllowAnonymous]
    [HttpGet("GetAll")]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Get()
    {
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        
        return Ok(await _characterService.GetAllCharacters(userId));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> GetSingle(int id)
    {
        return Ok(await _characterService.GetCharacterById(id));
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Create(AddCharacterDto character)
    {
        return Ok(await _characterService.AddCharacter(character));
    }
    
    [HttpPut]
    public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> Update(UpdateCharacterDto character)
    {
        var response = await _characterService.UpdateCharacter(character);
        if (response.Success) return Ok(response);
        else return NotFound(response);
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Delete(int id)
    {
        var response = await _characterService.DeleteCharacter(id);
        if (response.Success) return Ok(response);
        else return NotFound(response);
    }
}