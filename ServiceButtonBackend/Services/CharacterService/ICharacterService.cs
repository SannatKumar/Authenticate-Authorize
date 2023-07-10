namespace ServiceButtonBackend.Services.CharacterService
{
    public interface ICharacterService
    {
        //Task is needed when async is used: ServiceResponse is from ServiceResponse Class to send messages
        Task<ServiceResponse<List<Character>>> GetAllCharacters();

        Task<ServiceResponse<Character>> GetCharacterById(int id);

        Task<ServiceResponse<List<Character>>> AddCharacter(Character newCharacter);


    }
}
