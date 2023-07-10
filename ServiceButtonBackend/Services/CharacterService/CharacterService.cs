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
        public List<Character> AddCharacter(Character newCharacter)
        {
            CharacterService.Add(newCharacter);
            return characters;
        }

        public List<Character> GetAllCharacters()
        {
            return characters;
        }

        public Character GetCharacterById(int id)
        {
            return characters.FirstOrDefault(c => c.Id == id);
        }
    }
}
