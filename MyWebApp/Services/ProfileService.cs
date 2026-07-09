using Microsoft.AspNetCore.Mvc;
using MyWebApp.Models;
using MyWebApp.SupabaseClient;
using Supabase;
using Supabase.Gotrue;

namespace MyWebApp.Services
{
    public static class ProfileService
    {
        static Supabase.Client client = SupabClient.GetSupabaseClient();

        public static async Task<string> CopyFileOnServerAsync(IFormFile photo, string userId)
        {
            string photoPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images",
                "avatar",
                $"{userId}.jpg");

            await using var stream = new FileStream(photoPath, FileMode.Create);
            await photo.CopyToAsync(stream);

            return photoPath;
        }

        public static async Task<string> UploadPhoto(IFormFile photo, string userId)
        {
            string filePath = await CopyFileOnServerAsync(photo, userId);
            string fileName = Path.GetFileName(filePath);
            byte[] fileBytes = await File.ReadAllBytesAsync(filePath);

            var storage = client.Storage.From("avatar");

            try
            {
                await storage.Upload(fileBytes, fileName);
            }
            catch
            {
                await storage.Update(fileBytes, fileName,
                    new Supabase.Storage.FileOptions { Upsert = true });
            }

            string publicUrl = storage.GetPublicUrl(fileName);

            await UpdatePhoto(userId, publicUrl);

            return publicUrl;
        }

        public static async Task UpdatePhoto(string userId, string publicUrl)
        {
            await client
                .From<Profile>()
                .Where(x => x.Id == userId)
                .Set(x => x.AvatarUrl, publicUrl)
                .Update();
        }
    }
}



