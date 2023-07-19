using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceButtonBackend.Models
{
    [Keyless]
    public class UserPermission
    {
        [Column("page_id")]
        public int PageId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("page_name")]
        public string? PageName { get; set; }

        [Column("read")]
        public int Read { get; set; }

        [Column("create")]
        public int Create { get; set; }

        [Column("update")]
        public int Update { get; set; }

        [Column("delete")]
        public int Delete { get; set; }



    }
}
