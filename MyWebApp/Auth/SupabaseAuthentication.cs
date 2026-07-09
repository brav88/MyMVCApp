using MyWebApp.SupabaseClient;
using Supabase;

namespace MyWebApp.Auth
{
    public static class SupabaseAuthentication
    {
        public static async Task<Supabase.Gotrue.Session> SignIn(string txtEmail, string txtPwd)
        {
            try
            {
                Client client = SupabClient.GetSupabaseClient();

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
