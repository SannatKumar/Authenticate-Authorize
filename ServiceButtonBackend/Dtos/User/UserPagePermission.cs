namespace ServiceButtonBackend.Dtos.User
{
    public class UserPagePermission
    {
        public Dictionary<string, PermissionDetails>? Entities { get; set; }
    }

    public class PermissionDetails
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
    }
}
