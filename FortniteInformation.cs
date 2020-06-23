using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FortniteInformation
{
    internal class FortniteInformation // to lazy to handle errors
    {

        private readonly HttpClient client = new HttpClient();

        public async Task StartAsync()
        {
            Print("Info", "Getting Fortnite information...");
            var login = await AuthenticateAsync();
            var manifest = await GetManifestAsync(login);
            var chunkManifest = await GetChunkManifestAsync(manifest);
            var paks = chunkManifest.FileManifestList.Where(file =>
                file.Filename.StartsWith("Fortnite") && file.Filename.EndsWith(".pak")
            );
            var dynamicPaks = paks.Where(file =>
            {
                var name = file.Filename.Split("/")[file.Filename.Split("/").Count() - 1];
                return name.StartsWith("pakchunk10", StringComparison.OrdinalIgnoreCase) && name.Length == 30;
            });
            Print("Info", $"App: {manifest.appName}", true);
            Print("Info", $"Catalog ID: {manifest.catalogItemId}");
            Print("Info", $"Build Version: {manifest.buildVersion}");

            Print("Pak", $"Pak Count: {paks.Count()}", true);
            Print("Pak", $"Dyanmic Pak Count: {dynamicPaks.Count()}");

            Console.Write("\nDo you want to see every pak file? [Y/n] ");
            var key = Console.ReadKey();
            Console.Write("\n");
            if (key.Key == ConsoleKey.Y)
            {
                foreach (var pak in paks)
                {
                    var name = pak.Filename.Split("/")[pak.Filename.Split("/").Count() - 1];
                    Print("Pak", name);
                }
            }

            await PrintAES();

            Print("Credits", "The one and only Thoo for making it", true);
            Print("Credits", "BenBot to grab the AES keys");

            Console.WriteLine("\n\nPress any key to close...");
            Console.ReadKey();
        }

        private async Task PrintAES()
        {
            var response = await client.GetAsync("https://benbotfn.tk/api/v1/aes");
            if (!response.IsSuccessStatusCode)
            {
                Print("AES", "Couldn't get AES Keys!");
                return;
            }
            var content = await response.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<AESModel>(content);
            if(deserialized.mainKey != null)
            {
                Print("AES", $"Main Key: {deserialized.mainKey}", true);
            } else
            {
                Print("AES", "Main Key: Unknown", true);
            }
            if(deserialized.dynamicKeys != null)
            {
                foreach(KeyValuePair<string, string> entry in deserialized.dynamicKeys)
                {
                    Print("AES", $"{entry.Key.Split("/")[entry.Key.Split("/").Length - 1]}: {entry.Value}");
                }
            }
        }

        private void Print(string tag, string txt, bool newline = false)
        {
            if(newline) Console.Write("\n");
            Console.Write($"[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(tag);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"] {txt}\n");
        }

        private async Task<ChunkManifest> GetChunkManifestAsync(AppManifest manifest)
        {
            var manifestItem = manifest.items["MANIFEST"];
            var request = new HttpRequestMessage(HttpMethod.Get, $"{manifestItem.distribution}{manifestItem.path}?{manifestItem.signature}");
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ChunkManifest>(content);
        }

        private async Task<AppManifest> GetManifestAsync(LoginResponse login)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Utils.MANIFEST_URL);
            request.Headers.Add("Authorization", $"bearer {login.AccessToken}");
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AppManifest>(content);
        }

        private async Task<LoginResponse> AuthenticateAsync()
        {
            var body = CreateBody<string>(
                Of("grant_type", "client_credentials"),
                Of("token_type", "eg1")
            );
            var request = new HttpRequestMessage(HttpMethod.Post, Utils.AUTH_URL);
            request.Headers.Add("Authorization", $"basic {Utils.EGS_TOKEN}");
            request.Content = new FormUrlEncodedContent(body);
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LoginResponse>(content);
        }

        private List<KeyValuePair<string, T>> CreateBody<T>(params KeyValuePair<string, T>[] pairs)
            => pairs.OfType<KeyValuePair<string, T>>().ToList();

        private KeyValuePair<string, T> Of<T>(string k, T v)
            => new KeyValuePair<string, T>(k, v);

    }
}
