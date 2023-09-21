using ObjectSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donkey.Administration.WebApi
{
    public class AddOrderModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime? FinishTime { get; set; }
        public string Description { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public string Resources { get; set; }


        public Order GetOrder() => new Order()
        {
            Status = OrderStatus.Created,
            ArrivalTime = DateTime.Now,
            Description = Description,
            FinishTime = FinishTime,
            Name = Name,
            Resources = Resources,
            Tags = Tags
        };

        public bool IsCorrect =>
            !string.IsNullOrEmpty(Email) &&
            !string.IsNullOrEmpty(Name);
    }
}
