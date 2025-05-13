using System.Collections.Generic;
using System.Threading.Tasks;

namespace CycleRetailShopAPI.Interfaces
{
    public interface IUserService
    {
        Task<List<string>> GetAdminEmails();
    }
}
