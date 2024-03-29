﻿using IdentityModel.OidcClient;
using NetCoreConsoleClient.Configuration;
using Serilog;
using System;
using System.Threading.Tasks;

namespace ConsoleClientWithBrowser
{
    public class Program
    {
        private static OidcClient _oidcClient;

        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        public static async Task MainAsync()
        {
            var configLoader = new ConfigLoader("config.yaml");
            var config = configLoader.LoadConfigFromFile();

            Console.WriteLine("OIDC-Config:");
            config.PrintToConsole();

            Console.WriteLine("\n");
            Console.WriteLine("+-----------------------+");
            Console.WriteLine("|  Sign in with OIDC    |");
            Console.WriteLine("+-----------------------+");
            Console.WriteLine("");
            Console.WriteLine("Press any key to sign in...");
            Console.ReadKey();

            await Login(config);
        }

        private static async Task Login(OidcClientOptions options)
        {
            // create a redirect URI using an available port on the loopback address. requires the
            // OP to allow random ports on 127.0.0.1 - otherwise set a static port
            var browser = new SystemBrowser(3000);
            string redirectUri = string.Format($"http://127.0.0.1:{browser.Port}");
            //string redirectUri = "https://keycloak.docufy.de/auth/realms/AzureAdTest/broker/oidc/endpoint/auth";

            //var options = new OidcClientOptions
            //{
            //    Authority = _authority,
            //    ClientId = "native.code",
            //    RedirectUri = redirectUri,
            //    Scope = "openid profile api",
            //    FilterClaims = false,
            //    Browser = browser,
            //    Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,
            //    ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect
            //};

            options.Browser = browser;
            options.RedirectUri = redirectUri;

            var serilog = new LoggerConfiguration()
                .MinimumLevel.Error()
                .Enrich.FromLogContext()
                .WriteTo.LiterateConsole(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}{Exception}{NewLine}")
                .CreateLogger();

            options.LoggerFactory.AddSerilog(serilog);

            _oidcClient = new OidcClient(options);
            var result = await _oidcClient.LoginAsync(new LoginRequest());

            ShowResult(result);
            await NextSteps(result);
        }

        private static async Task NextSteps(LoginResult result)
        {
            var currentAccessToken = result.AccessToken;
            var currentRefreshToken = result.RefreshToken;

            var menu = "  x...exit   ";
            if (currentRefreshToken != null) menu += "r...refresh token   ";

            while (true)
            {
                Console.WriteLine("\n\n");

                Console.Write(menu);
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.X) return;
                if (key.Key == ConsoleKey.R)
                {
                    var refreshResult = await _oidcClient.RefreshTokenAsync(currentRefreshToken);
                    if (refreshResult.IsError)
                    {
                        Console.WriteLine($"Error: {refreshResult.Error}");
                    }
                    else
                    {
                        currentRefreshToken = refreshResult.RefreshToken;
                        currentAccessToken = refreshResult.AccessToken;

                        Console.WriteLine("\n\n");
                        Console.WriteLine($"access token:   {refreshResult.AccessToken}");

                        Console.WriteLine($"refresh token:  {refreshResult?.RefreshToken ?? "none"}");
                    }
                }
            }
        }

        private static void ShowResult(LoginResult result)
        {
            if (result.IsError)
            {
                Console.WriteLine("\n\nError:\n{0}", result.Error);
                return;
            }

            Console.WriteLine("\n\nClaims:");
            foreach (var claim in result.User.Claims)
            {
                Console.WriteLine("{0}: {1}", claim.Type, claim.Value);
            }

            Console.WriteLine("\n");
            Console.WriteLine($"identity token: \n{result.IdentityToken}");
            Console.WriteLine("\n");
            Console.WriteLine($"access token: \n{result.AccessToken}");
            Console.WriteLine("\n");
            Console.WriteLine($"refresh token: \n{result?.RefreshToken ?? "none"}");
        }
    }
}