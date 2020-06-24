using IdentityModel.OidcClient;
using System;
using System.IO;
using System.Reflection;
using YamlDotNet.Serialization;

namespace NetCoreConsoleClient.Configuration
{
    public class ConfigLoader
    {
        private readonly string configfileFullPath;
        private readonly string directoryPath;
        private readonly string fileName;

        public ConfigLoader(string fileName)
        {
            this.fileName = fileName;
            directoryPath = Environment.CurrentDirectory;
            configfileFullPath = Path.Combine(directoryPath, fileName);
        }

        public OidcClientOptions LoadConfigFromFile()
        {
            var input = new StreamReader(configfileFullPath);
            var deserializer = new Deserializer();
            var config = deserializer.Deserialize<Config>(input);

            var oidcOptions = new OidcClientOptions();

            if (config != null)
            {
                oidcOptions.Authority = config.IssuerUrl;
                oidcOptions.Scope = config.Scope;
                oidcOptions.ClientId = config.ClientId;
                oidcOptions.FilterClaims = false;
                oidcOptions.Policy.RequireAccessTokenHash = false;
                oidcOptions.Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode;
                oidcOptions.ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect;
            }

            return oidcOptions;
        }
    }
}