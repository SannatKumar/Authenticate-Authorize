
using Microsoft.AspNetCore.Http.HttpResults;
using ServiceButtonBackend.Models;

namespace ServiceButtonBackend.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        //private readonly List<Character> characters = new List<Character>
        //{
        //    new Character(),
        //    new Character() {Id = 1, Name = "Sam"}
        //};

        //For AutoMapper
        private readonly IMapper _mapper;
        //For Data Context
        private readonly DataContext _context;

        public CharacterService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<ServiceRespone<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceRespone<List<GetCharacterDto>>();
            var character = _mapper.Map<Character>(newCharacter);

            _context.Characters.Add(character);
            await _context.SaveChangesAsync();

            serviceResponse.Data = await _context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceRespone<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceRespone<List<GetCharacterDto>>();
            try
            {

                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
                if (character is null)
                {
                    throw new Exception($"Character with Id '{id}' not found.");
                }

                _context.Characters.Remove(character);

                await _context.SaveChangesAsync();

                serviceResponse.Data = await _context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceRespone<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceRespone<List<GetCharacterDto>>();
            var dbCharacters = await _context.Characters.ToListAsync();
            serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return serviceResponse;
        }

        //public Character? GetCharacterById(int id)
        public async Task<ServiceRespone<GetCharacterDto>> GetCharacterById(int id)
        {
            //return characters.FirstOrDefault(c => c.Id == id)!;
            var serviceResponse = new ServiceRespone<GetCharacterDto>();
            var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);

            serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);

            return serviceResponse;

        }

        public async Task<ServiceRespone<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            var serviceResponse = new ServiceRespone<GetCharacterDto>();
            try
            {
                
                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
                if(character is null)
                {
                    throw new Exception($"Character with Id '{updatedCharacter.Id}' not found.");
                }

                /*
                //Another Options

                _mapper.Map(updatedCharacter, character);
                */

                character.Name = updatedCharacter.Name;
                character.HitPoints = updatedCharacter.HitPoints;
                character.Strength = updatedCharacter.Strength;
                character.Defence = updatedCharacter.Defence;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Class = updatedCharacter.Class;


                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch (Exception ex) 
            {
                serviceResponse.Success = false;
                serviceResponse.Message =  ex.Message;
            }

            return serviceResponse;


        }
    }
}
