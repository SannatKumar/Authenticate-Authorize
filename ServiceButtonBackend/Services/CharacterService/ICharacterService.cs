namespace ServiceButtonBackend.Services.CharacterService
{
    public interface ICharacterService
    {
        //Task is needed when async is used: ServiceResponse is from ServiceResponse Class to send messages
        Task<ServiceRespone<List<GetCharacterDto>>> GetAllCharacters();

        Task<ServiceRespone<GetCharacterDto>> GetCharacterById(int id);

        Task<ServiceRespone<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter);

        Task<ServiceRespone<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter);

        Task<ServiceRespone<List<GetCharacterDto>>> DeleteCharacter(int id);
    }
}
