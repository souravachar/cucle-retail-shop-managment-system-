using CycleRetailShopAPI.Models;
using Microsoft.AspNetCore.Http;  // ✅ Add this for IFormFile
using System.Collections.Generic;

namespace CycleRetailShopAPI.Interfaces
{
    public interface ICycleService
    {
        List<Cycle> GetAllCycles();
        Cycle GetCycleById(int id);
        Cycle AddCycle(Cycle cycle, IFormFile? imageFile); // ✅ Added Image File Parameter
        Cycle UpdateCycle(int id, Cycle updatedCycle, IFormFile? imageFile);
        bool DeleteCycle(int id);
        bool UpdateStockQuantity(int id, int quantity);
    }
}
