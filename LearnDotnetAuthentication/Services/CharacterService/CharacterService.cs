namespace LearnDotnetAuthentication.Services.CharacterService;

public class CharacterService: ICharacterService
{
    private readonly IMapper _mapper;
    private readonly DataContext _dataContext;

    public CharacterService(IMapper mapper, DataContext dataContext)
    {
        _mapper = mapper;
        _dataContext = dataContext;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters(int userId)
    {
        var dbCharacters = await _dataContext.Characters.Where(c => c.User.Id == userId).ToListAsync();
        var serviceResponse = new ServiceResponse<List<GetCharacterDto>>
        {
            Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList()
        };

        return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
    {
        var serviceResponse = new ServiceResponse<GetCharacterDto>();
        var character = await _dataContext.Characters.FirstOrDefaultAsync(c => c.Id == id);
        serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
        return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto character)
    {
        var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
        var newChar = _mapper.Map<Character>(character);
        _dataContext.Characters.Add(newChar);
        await _dataContext.SaveChangesAsync();
        serviceResponse.Data = await _dataContext.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
        return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto character)
    {
        var serviceResponse = new ServiceResponse<GetCharacterDto>();
        try
        {
            var existChar = await _dataContext.Characters.FirstOrDefaultAsync(c => c.Id == character.Id);

            if (existChar is null) throw new Exception($"Character with Id '{character.Id}' not found");

            _mapper.Map(character, existChar);

            await _dataContext.SaveChangesAsync();

            serviceResponse.Data = _mapper.Map<GetCharacterDto>(existChar);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
    {
        var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
        try
        {
            var character = await _dataContext.Characters.FirstOrDefaultAsync(c => c.Id == id);
            
            if (character is null) 
                throw new Exception($"Character with Id '{id}' not found");

            _dataContext.Characters.Remove(character);

            await _dataContext.SaveChangesAsync();

            serviceResponse.Data = await _dataContext.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }
        return serviceResponse;
    }
}