using ObjectSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Donkey.Administration.Data.DataModel
{
    public class Order
    {
        [Key]
        public string Guid { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public Client Client { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        public DateTime? FinishTime { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        public Admin LastUpdateAdmin { get; set; }

        public Tag[] Tags { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        public double? PercentCompleted { get; set; }

        public string Resources { get; set; }
    }
}
