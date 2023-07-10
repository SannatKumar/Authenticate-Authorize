
using Microsoft.AspNetCore.Mvc;


namespace ServiceButtonBackend.Controllers
{
    [ApiController]
    [Route("api/v-1")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet("getall")]
        
        public ActionResult<Character> Get()
        {
            return Ok(_characterService.GetAllCharacters());
        }

        [HttpGet("getsingle")]
        public ActionResult<Character> GetSingle(int id)
        {
            return Ok(_characterService.GetCharacterById(id));
        }

        [HttpPost("character/add")]
        public ActionResult<List<Character>> AddCharacter(Character newCharacter)
        {
            return Ok(_characterService.AddCharacter(newCharacter));
        }




    }
}
