﻿namespace ServiceButtonBackend.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public byte[] PasswordHash { get; set; } = new byte[0];

        public byte[] PasswordSalt { get; set; } = new byte[0];

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime TokenCreated { get; set; } 
        public DateTime TokenExpires{ get; set; }
        
        public List<Character>?  Characters{ get; set; }
    }
}
