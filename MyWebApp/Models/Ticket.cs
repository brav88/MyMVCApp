using MyWebApp.SupabaseClient;
using Supabase;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Responses;

namespace MyWebApp.Models
{
    [Table("tickets")]
    public class Ticket : BaseModel
    {
        [PrimaryKey("id", false)] 
        public long Id { get; set; }

        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("status")]
        public string Status { get; set; } = "Open";

        [Column("priority")]
        public string Priority { get; set; } = "Medium";

        [Column("ticket_type")]
        public string TicketType { get; set; } = "Task";

        [Column("created_by")]
        public string? CreatedBy { get; set; }

        [Column("assigned_to")]
        public string? AssignedTo { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("due_date")]
        public DateTime? DueDate { get; set; }

        public List<CommentProfiles>? Comments { get; set; }

        public string? ActiveSessionUserId { get; set; }
    }

    [Table("comments")]
    public class Comment : BaseModel
    {
        [PrimaryKey("id", false)] // false indica que es auto-generado por la BD (Identity)
        public long Id { get; set; }

        [Column("ticket_id")]
        public long TicketId { get; set; }

        [Column("comment_text")]
        public string? CommentText { get; set; }

        [Column("created_by")]
        public string? CreatedBy { get; set; } // O puedes usar Guid? si prefieres tipado estricto para el UUID

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    [Table("v_comments_with_profiles")]
    public class CommentProfiles : BaseModel
    {
        [PrimaryKey("comment_id", false)] // Indicamos que es la llave, el "false" es porque no es auto-generada al insertar en una vista
        public long CommentId { get; set; }

        [Column("ticket_id")]
        public long TicketId { get; set; }

        [Column("comment_text")] // Mantiene el typo exacto de tu base de datos (con tres 'm')
        public string? CommentText { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("user_id")]
        public string? UserId { get; set; } // UUID de Postgres se mapea como string o Guid

        [Column("avatar_url")]
        public string? AvatarUrl { get; set; }

        [Column("full_name")]
        public string? FullName { get; set; }

        [Column("user_email")]
        public string? UserEmail { get; set; }
    }
}

  