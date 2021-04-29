using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModelUI.Utilities
{
    public static class KeyVaultService 
    {

        public static string GetValueSecretFromKeyVault(string keyVaultName, string secretName)
        {
            if (keyVaultName == null)
            {
                throw new ArgumentNullException(nameof(keyVaultName));
            }

            if (secretName == null)
            {
                throw new ArgumentNullException(nameof(secretName));
            }

            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            var keyVaultClient =
                new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

            var secretAsync = keyVaultClient.GetSecretAsync(keyVaultName, secretName);

            return secretAsync.Result.Value;
        }

    }
}
