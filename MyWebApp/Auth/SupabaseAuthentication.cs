using Supabase;

namespace MyWebApp.Auth
{
    public static class SupabaseAuthentication
    {
        private static string url = "";
        private static string key = "";

        public static async Task<Supabase.Gotrue.Session> SignIn(string txtEmail, string txtPwd)
        {
            try
            {
                var client = new Client(url, key);

                await client.InitializeAsync();

                Supabase.Gotrue.Session? session = await client.Auth.SignIn(txtEmail, txtPwd);

                return session;
            }
            catch
            {
                return null;
            }
        }
    }
}
