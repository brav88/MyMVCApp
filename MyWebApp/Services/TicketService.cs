using MyWebApp.Models;
using MyWebApp.SupabaseClient;
using Supabase;

namespace MyWebApp.Services
{
    public static class TicketService
    {
        public static async Task<List<Ticket>> getAll()
        {
            Client client = SupabClient.getSupabaseClient();

            await client.InitializeAsync();

            var result = await client.From<Ticket>().Get();
            return result.Models;
        }

        public static async Task<Ticket> getTicketById(int id)
        {
            Client client = SupabClient.getSupabaseClient();

            await client.InitializeAsync();

            var result = await client.From<Ticket>().Where(x => x.Id == id).Get();

            var comments = await client.From<Comment>().Where(x => x.TicketId == id).Get();

            result.Model?.Comments = comments.Models;

            return result.Model;
        }

        public static async void postComment(Comment comment)
        {
            Client client = SupabClient.getSupabaseClient();

            await client.InitializeAsync();

            var result = await client.From<Comment>().Insert(comment);          
        }
    }
}
