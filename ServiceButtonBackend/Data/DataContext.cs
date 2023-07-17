﻿using ServiceButtonBackend.Dtos.User;
namespace ServiceButtonBackend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {
        }

        public DbSet<Character> Characters => Set<Character>();

        public DbSet<User> Users => Set<User>();

        public DbSet<UserRefreshTokenDto> UserRefreshToken => Set<UserRefreshTokenDto>();

        public DbSet<UserDetail> UserDetails => Set<UserDetail>();

        public DbSet<UserPagePermission> vUserPermissions => Set<UserPagePermission>();
    }
}
