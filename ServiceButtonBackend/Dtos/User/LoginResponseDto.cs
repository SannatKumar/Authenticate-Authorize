namespace ServiceButtonBackend.Dtos.User
{
    public class LoginResponseDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Locale { get; set; } = string.Empty;

    }
}
