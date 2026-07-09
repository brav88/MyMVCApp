using MyWebApp.Models;
using MyWebApp.SupabaseClient;
using Supabase;
using System.Xml.Linq;

namespace MyWebApp.Services
{
    public static class TicketService
    {
        static Client client = SupabClient.GetSupabaseClient();

        public static async Task<List<Ticket>> GetAll()
        {
            await client.InitializeAsync();

            return (await client.From<Ticket>().Get()).Models;
        }

        public static async Task<Ticket> GetTicketById(int id)
        {
            await client.InitializeAsync();

            var ticket = (await client.From<Ticket>().Where(x => x.Id == id).Get()).Model;

            if (ticket is not null)
                ticket.Comments = (await client.From<Comment>().Where(x => x.TicketId == id).Get()).Models;
            
            return ticket;           
        }

        public static async void PostComment(Comment comment)
        {
            await client.InitializeAsync();

            await client.From<Comment>().Insert(comment);
        }

        public static async void DeleteComment(int commentId)
        {
            await client.InitializeAsync();

            await client.From<Comment>().Where(x => x.Id == commentId).Delete();
        }
    }
}
