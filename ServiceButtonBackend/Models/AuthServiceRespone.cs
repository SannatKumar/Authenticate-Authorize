using ServiceButtonBackend.Dtos.User;

namespace ServiceButtonBackend.Models
{
    public class AuthServiceRespone<T>
    {
        public T? Data { get; set; }

        public LoginResponseDto? UserDetail { get; set; }

        public List<UserPermission>? UserPermisssion { get; set; }

        public bool Success { get; set; } = true;

        public string Message { get; set; } = string.Empty;
    }
}
