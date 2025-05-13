﻿using System.ComponentModel.DataAnnotations;

namespace CycleRetailShopAPI.DTOs
{
    public class OrderDetailDTO
    {
        [Required]
        public int CycleID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}
