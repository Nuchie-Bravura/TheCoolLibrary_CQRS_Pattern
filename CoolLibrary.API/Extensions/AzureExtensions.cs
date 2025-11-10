using Azure.Identity;


namespace CoolLibrary.API
{
    public static class AzureExtensions
    {
        public static IConfigurationBuilder AddAzureKeyVaultIfConfigured(this IConfigurationBuilder configuration, IHostEnvironment env)
        {
            var builtConfig = configuration.Build();
            var keyVaultUrl = builtConfig["KeyVault:Url"];

            if (!string.IsNullOrWhiteSpace(keyVaultUrl) && !env.IsDevelopment())
            {
                try
                {
                    configuration.AddAzureKeyVault(new Uri(keyVaultUrl), new DefaultAzureCredential());
                    Console.WriteLine($"Azure Key Vault loaded from {keyVaultUrl}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[KeyVault] Could not load Key Vault: {ex.Message}");
                }
            }

            return configuration;
        }
    }
}
