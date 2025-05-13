// File: Interfaces/IEmailService.cs
using System.Threading.Tasks;

namespace CycleRetailShopAPI.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailWithAttachmentAsync(string[] to, string subject, string body, byte[] attachment, string filename);
    }
}


