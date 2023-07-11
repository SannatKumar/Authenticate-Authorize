namespace ServiceButtonBackend
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>();
            CreateMap<AddCharacterDto, Character>();

            /* 
             * this is necessary when update is done with Automapper
            CreateMap<UpdateCharacterDto, Character>();
            */
        }
    }
}
