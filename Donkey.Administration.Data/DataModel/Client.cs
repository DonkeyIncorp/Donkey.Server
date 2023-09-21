using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Donkey.Administration.Data.DataModel
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string SecondName { get; set; }

        public string ThirdName { get; set; }

        [Required]
        public string Email { get; set; }

        public string Contacts { get; set; }


        public ICollection<Order> Orders { get; set; }

    }
}
