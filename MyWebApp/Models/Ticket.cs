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

        public List<Comment>? Comments { get; set; }

        public string ActiveSessionUserId { get; set; }
    }

    [Table("comments")]
    public class Comment : BaseModel
    {
        [PrimaryKey("id", false)] // false indica que es auto-generado por la BD (Identity)
        public long Id { get; set; }

        [Column("ticket_id")]
        public long TicketId { get; set; }

        [Column("commment_text")]
        public string? CommentText { get; set; }

        [Column("created_by")]
        public string? CreatedBy { get; set; } // O puedes usar Guid? si prefieres tipado estricto para el UUID

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}

  