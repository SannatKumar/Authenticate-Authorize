using ServiceButtonBackend.Dtos.User;

namespace ServiceButtonBackend.Models
{
    public class AuthServiceRespone<T>
    {
        //public T? Data { get; set; }

        public T? token { get; set; }

        public LoginResponseDto? UserDetail { get; set; }

        public List<UserPermission>? UserPermission { get; set; }

        public bool Success { get; set; } = true;

        public string Message { get; set; } = string.Empty;
    }
}
