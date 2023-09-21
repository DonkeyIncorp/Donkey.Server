using ObjectSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Donkey.Administration.WebApi
{
    public class UpdateOrderModel
    {
        [Required(ErrorMessage = "Идетнтификатор должен быть указан")]
        public string Guid { get; set; }
        
        [Required(ErrorMessage = "Название заказа должно быть указано")]
        public string Name { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public string Description { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public OrderStatus Status { get; set; }
        public string Resources { get; set; }
        public double? PercentCompleted { get; set; }

        public Order GetOrder() => new Order()
        {
            Guid = Guid,
            Name = Name,
            Status = Status,
            ArrivalTime = ArrivalTime,
            Description = Description,
            FinishTime = FinishTime,
            PercentCompleted = PercentCompleted,
            Resources = Resources,
            Tags = Tags
        };
    }
}
