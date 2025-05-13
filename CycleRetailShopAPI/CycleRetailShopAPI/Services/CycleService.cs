using CycleRetailShopAPI.Interfaces;
using CycleRetailShopAPI.Models;
using CycleRetailShopAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;  // ✅ For JSON Parsing
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CycleRetailShopAPI.Services
{
    public class CycleService : ICycleService
    {
        private readonly CycleRepository _cycleRepository;

        public CycleService(CycleRepository cycleRepository)
        {
            _cycleRepository = cycleRepository;
        }

        public List<Cycle> GetAllCycles()
        {
            return _cycleRepository.GetAll();
        }

        public Cycle GetCycleById(int id)
        {
            return _cycleRepository.GetById(id);
        }

        public Cycle AddCycle(Cycle cycle, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                imageFile.CopyTo(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();
                string base64Image = Convert.ToBase64String(fileBytes);

                string imgbbApiKey = "dfd91e08cf9d648eb35c41aaf73f45ca";
                string uploadUrl = $"https://api.imgbb.com/1/upload?key={imgbbApiKey}";

                using var httpClient = new HttpClient();
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(base64Image), "image");

                var response = httpClient.PostAsync(uploadUrl, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                    cycle.ImageUrl = result["data"]["url"];
                }
            }

            cycle.Description = cycle.Description ?? "No description provided.";
            cycle.UpdatedAt = DateTime.UtcNow;  // ✅ Set UpdatedAt on creation

            return _cycleRepository.Add(cycle);
        }

        public Cycle UpdateCycle(int id, Cycle updatedCycle, IFormFile? imageFile)
        {
            var existingCycle = _cycleRepository.GetById(id);
            if (existingCycle == null) return null;

            existingCycle.ModelName = updatedCycle.ModelName;
            existingCycle.Brand = updatedCycle.Brand;
            existingCycle.Type = updatedCycle.Type;
            existingCycle.Price = updatedCycle.Price;
            existingCycle.StockQuantity = updatedCycle.StockQuantity;
            existingCycle.Description = updatedCycle.Description;
            existingCycle.UpdatedAt = DateTime.UtcNow;  // ✅ Set UpdatedAt on update

            if (imageFile != null && imageFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                imageFile.CopyTo(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();
                string base64Image = Convert.ToBase64String(fileBytes);

                string imgbbApiKey = "dfd91e08cf9d648eb35c41aaf73f45ca";
                string uploadUrl = $"https://api.imgbb.com/1/upload?key={imgbbApiKey}";

                using var httpClient = new HttpClient();
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(base64Image), "image");

                var response = httpClient.PostAsync(uploadUrl, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                    existingCycle.ImageUrl = result["data"]["url"];
                }
            }

            return _cycleRepository.Update(id, existingCycle);
        }



        public bool DeleteCycle(int id)
        {
            return _cycleRepository.Delete(id);
        }

        public bool UpdateStockQuantity(int id, int quantity)
        {
            return _cycleRepository.UpdateStock(id, quantity);
        }
    }
}
