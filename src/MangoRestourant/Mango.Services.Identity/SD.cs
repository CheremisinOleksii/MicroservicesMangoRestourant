using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace Mango.Services.Identity
{
    public static class SD
    {
        public const string ADMIN = "Admin";
        public const string CUSTOMER = "Customer";

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource> {
            new IdentityResources.OpenId(),
            new IdentityResources.Email(),
            new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>
        {
            new ApiScope(name: "mango",displayName: "Mango Server"),
            new ApiScope(name: "read",displayName: "Read your data"),
            new ApiScope(name: "write",displayName: "Write your data"),
            new ApiScope(name: "delete",displayName: "Delete your data")
        };

        public static IEnumerable<Client> Clients => new List<Client> {
            new Client {
                ClientId = "client",
                ClientSecrets =  { new Secret("secret".Sha256())},
                AllowedGrantTypes = {GrantType.ClientCredentials },
                AllowedScopes = {"read", "write", "profile" }
            },
             new Client {
                ClientId = "mango",
                ClientSecrets =  { new Secret("secret".Sha256())},
                AllowedGrantTypes = {GrantType.AuthorizationCode },
                RedirectUris = { "https://localhost:44353/signin-oidc" },
                PostLogoutRedirectUris = { "https://localhost:44353/signout-callback-oidc" },
                AllowedScopes = new List<string>{
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "mango"
                }
            }
        };
    }
}
