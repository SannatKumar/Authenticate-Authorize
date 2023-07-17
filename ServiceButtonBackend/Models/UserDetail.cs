using System.ComponentModel.DataAnnotations;

namespace ServiceButtonBackend.Models
{
    public class UserDetail
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(5)]
        public string Locale { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Role { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;

        public int Pin { get; set; }

        public int CreatorId { get; set; }

        public int UserId { get; set; }



    }
}
