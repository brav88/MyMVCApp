using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class CreditCard
    {
        [Key]
        public int Id { get; set; }        
        public string? Number { get; set; }
        public string? Holder { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int CVV { get; set; }
        public string? Issuer { get; set; }
        public char Status { get; set; }
    }
}
