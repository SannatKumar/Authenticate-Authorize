namespace ServiceButtonBackend.Services.CharacterService
{
    public interface ICharacterService
    {
        //Task is needed when async is used: ServiceResponse is from ServiceResponse Class to send messages
        Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters();

        Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id);

        Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter);


    }
}
