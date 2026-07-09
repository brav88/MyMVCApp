using Microsoft.AspNetCore.Mvc;
using MyWebApp.Models;
using MyWebApp.SupabaseClient;
using Supabase;
using Supabase.Gotrue;
using static System.Collections.Specialized.BitVector32;

namespace MyWebApp.Services
{
    public static class ProfileService
    {
        static Supabase.Client client = SupabClient.getSupabaseClient();

        public static string CopyFileOnServer(IFormFile photo, string userId)
        {
            string photoPath = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\avatar\\" + userId + ".jpg";

            using (var stream = new FileStream(photoPath, FileMode.Create))
            {
                photo.CopyTo(stream);
            }

            return photoPath;
        }

        public static async Task<string> UploadPhoto(IFormFile photo, string userId)
        {
            try
            {
                string filePath = CopyFileOnServer(photo, userId);
                string fileName = Path.GetFileName(filePath);

                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

                try
                {
                    var response = await client.Storage
                    .From("avatar")
                    .Upload(fileBytes, fileName);
                }
                catch
                {
                    var response = await client.Storage
                    .From("avatar")
                    .Update(fileBytes, fileName, new Supabase.Storage.FileOptions { Upsert = true });
                }

                string publicUrl = client.Storage
                    .From("avatar")
                    .GetPublicUrl(fileName);

                await UpdatePhoto(userId, publicUrl);

                return publicUrl;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static async Task<bool> UpdatePhoto(string userId, string publicUrl)
        {
            try
            {
                var update = await client
                .From<Profile>()
                .Where(x => x.Id == userId)
                .Set(x => x.AvatarUrl, publicUrl)
                .Update();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}



