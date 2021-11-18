using System.Threading.Tasks;

namespace BFYOC.Functions
{
    public interface IBackEndService
    {
        Task<bool> CheckProductId(string productId);
        Task<bool> CheckUserId(string userId);
    }
}