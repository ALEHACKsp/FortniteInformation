using Newtonsoft.Json;
using System;

namespace FortniteInformation
{
    internal class LoginResponse
    {
        [JsonProperty("access_token")] public string AccessToken;
        [JsonProperty("expires_in")] public int ExpiresIn;
        [JsonProperty("expires_at")] public DateTime ExpiresAt;
        [JsonProperty("token_type")] public string TokenType;
        [JsonProperty("client_id")] public string ClientId;
        [JsonProperty("internal_client")] public bool InternalClient;
        [JsonProperty("client_service")] public string ClientService;

    }
}
