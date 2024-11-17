﻿using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new("auctionApp", "Auction App Full Access")
    ];

    public static IEnumerable<Client> Clients =>
    [
        new()
        {
            ClientId = "postman",
            ClientName = "Postman",
            AllowedScopes = { "openid", "profile", "auctionApp" },
            RedirectUris = { "https://www.getpostman.com/oauth2/callback" },
            ClientSecrets = { new Secret("NotASecret".Sha256()) },
            AllowedGrantTypes = { GrantType.ResourceOwnerPassword }
        }
    ];
}