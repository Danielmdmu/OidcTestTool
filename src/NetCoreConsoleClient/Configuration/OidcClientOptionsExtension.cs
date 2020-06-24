using IdentityModel.OidcClient;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetCoreConsoleClient.Configuration
{
    public static class OidcClientOptionsExtension
    {
        public static void PrintToConsole(this OidcClientOptions oidcClientOptions)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"Issuer-Url: {oidcClientOptions.Authority}");
            Console.WriteLine($"Scope: {oidcClientOptions.Scope}");
            Console.WriteLine($"ClientId: {oidcClientOptions.ClientId}");

            Console.ResetColor();
        }
    }
}