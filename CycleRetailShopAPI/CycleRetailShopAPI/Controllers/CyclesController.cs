using CycleRetailShopAPI.Interfaces;
using CycleRetailShopAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;  // ✅ Add this for IFormFile
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CycleRetailShopAPI.Controllers
{
    [Route("api/cycles")]
    [ApiController]
    public class CyclesController : ControllerBase
    {
        private readonly ICycleService _cycleService;

        public CyclesController(ICycleService cycleService)
        {
            _cycleService = cycleService;
        }


        [Authorize]
        [HttpPost("add-cycle")]
        public ActionResult<Cycle> AddCycle([FromForm] Cycle cycle, IFormFile? imageFile)
        {
            var newCycle = _cycleService.AddCycle(cycle, imageFile);
            return CreatedAtAction(nameof(GetCycleById), new { id = newCycle.CycleID }, newCycle);
        }

        [Authorize]
        [HttpPut("update-cycle/{id}")]
        public ActionResult<Cycle> UpdateCycle(int id, [FromForm] Cycle updatedCycle, IFormFile? imageFile)
        {
            var cycle = _cycleService.UpdateCycle(id, updatedCycle, imageFile);
            if (cycle == null) return NotFound();
            return Ok(cycle);
        }


        // ✅ Get All Cycles
        [Authorize]
        [HttpGet("get-all-cycles")]
        public ActionResult<List<Cycle>> GetAllCycles()
        {
            return Ok(_cycleService.GetAllCycles());
        }

        // ✅ Get Cycle by ID
        [Authorize]
        [HttpGet("get-cycle-by-id/{id}")]
        public ActionResult<Cycle> GetCycleById(int id)
        {
            var cycle = _cycleService.GetCycleById(id);
            if (cycle == null) return NotFound();
            return Ok(cycle);
        }



        // ✅ Delete a Cycle
        [Authorize]
        [HttpDelete("delete-cycle/{id}")]
        public IActionResult DeleteCycle(int id)
        {
            var success = _cycleService.DeleteCycle(id);
            if (!success) return NotFound();
            return NoContent();
        }

        // ✅ Update Stock Quantity
        [Authorize]
        [HttpPatch("add-stock/{id}/stock")]
        public IActionResult UpdateStockQuantity(int id, [FromBody] int quantity)
        {
            var success = _cycleService.UpdateStockQuantity(id, quantity);
            if (!success) return NotFound();
            return Ok(new { message = "Stock updated successfully" });
        }
    }
}
