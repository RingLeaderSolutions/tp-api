using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System.Configuration;
using System.Threading.Tasks;

namespace Theta.Platform.Common.SecretManagement
{
    public class Vault : IVault
    {
        private readonly string _vaultName;

        public Vault(string vaultName)
        {
            _vaultName = vaultName;
        }

        public async Task<string> GetSecret(string secretName)
        {
            // Useful if working locally and want to bypass the Azure Key Vault - just define secrets with same name in appSettings
            var appSetting = ConfigurationManager.AppSettings.Get(secretName);
            if (appSetting != null)
            {
                return appSetting;
            }

            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var secretBundle = await kv.GetSecretAsync($"https://{_vaultName}.vault.azure.net/", secretName);

            return secretBundle.Value;
        }
    }
}
