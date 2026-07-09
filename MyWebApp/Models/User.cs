using Microsoft.SqlServer.Server;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyWebApp.Models
{
    public class UserModel
    {
        public string? Email { get; set; }
        public string? Pwd { get; set; }
    }


    [Table("profiles")]
    public class Profile : BaseModel
    {
        [PrimaryKey("id", false)] 
        public string? Id { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("full_name")]
        public string? FullName { get; set; }

        [Column("avatar_url")]
        public string? AvatarUrl { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
