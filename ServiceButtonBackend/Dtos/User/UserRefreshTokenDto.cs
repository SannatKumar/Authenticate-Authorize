namespace ServiceButtonBackend.Dtos.User
{
    public class UserRefreshTokenDto
    {
        public string RefreshToken { get; set; } = string.Empty;

        public int UserId { get; set; }

        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpiresAt { get; set; }
    }
}
