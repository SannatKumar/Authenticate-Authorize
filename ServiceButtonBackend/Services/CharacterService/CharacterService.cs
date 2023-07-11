using Microsoft.AspNetCore.Http.HttpResults;
using ServiceButtonBackend.Models;

namespace ServiceButtonBackend.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly List<Character> characters = new List<Character>
        {
            new Character(),
            new Character() {Id = 1, Name = "Sam"}
        };
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            characters.Add(newCharacter);
            serviceResponse.Data = characters;
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            serviceResponse.Data = characters;
            return serviceResponse;
        }

        //public Character? GetCharacterById(int id)
        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            //return characters.FirstOrDefault(c => c.Id == id)!;
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            var character = characters.FirstOrDefault(c => c.Id == id);

            serviceResponse.Data = character;

            return serviceResponse;

        }
    }
}
