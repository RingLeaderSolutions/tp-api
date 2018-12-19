using System.Threading.Tasks;

namespace Theta.Platform.Common.SecretManagement
{
    public interface IVault
    {
        Task<string> GetSecret(string secretName);
    }
}
