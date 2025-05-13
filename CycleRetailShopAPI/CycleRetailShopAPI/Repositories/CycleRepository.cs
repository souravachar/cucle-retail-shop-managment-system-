using CycleRetailShopAPI.Data;
using CycleRetailShopAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace CycleRetailShopAPI.Repositories
{
    public class CycleRepository
    {
        private readonly ApplicationDbContext _context;

        public CycleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Cycle> GetAll()
        {
            return _context.Cycles.ToList();
        }

        public Cycle GetById(int id)
        {
            return _context.Cycles.Find(id);
        }

        public Cycle Add(Cycle cycle)
        {
            _context.Cycles.Add(cycle);
            _context.SaveChanges();
            return cycle;
        }

        public Cycle Update(int id, Cycle updatedCycle)
        {
            var cycle = _context.Cycles.Find(id);
            if (cycle == null) return null;

            cycle.ModelName = updatedCycle.ModelName;
            cycle.Brand = updatedCycle.Brand;
            cycle.Type = updatedCycle.Type;
            cycle.Price = updatedCycle.Price;
            cycle.StockQuantity = updatedCycle.StockQuantity;

            _context.SaveChanges();
            return cycle;
        }

        public bool Delete(int id)
        {
            var cycle = _context.Cycles.Find(id);
            if (cycle == null) return false;

            _context.Cycles.Remove(cycle);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateStock(int id, int quantity)
        {
            var cycle = _context.Cycles.Find(id);
            if (cycle == null) return false;

            cycle.StockQuantity = quantity;
            _context.SaveChanges();
            return true;
        }
    }
}
