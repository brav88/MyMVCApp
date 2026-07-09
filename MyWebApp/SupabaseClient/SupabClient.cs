using Supabase;

namespace MyWebApp.SupabaseClient
{
    public static class SupabClient
    {
        private static string Url = "";
        private static string Key = "";

        public static Client GetSupabaseClient() => new(Url, Key);
    }
}
