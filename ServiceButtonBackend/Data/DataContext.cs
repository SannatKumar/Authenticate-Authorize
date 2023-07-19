using ServiceButtonBackend.Dtos.User;

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

#pragma warning disable IDE1006 // Naming Styles
        public DbSet<UserPermission> v_user_permission => Set<UserPermission>();
#pragma warning restore IDE1006 // Naming Styles
    }
}
