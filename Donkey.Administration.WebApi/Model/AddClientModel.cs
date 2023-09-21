using ObjectSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donkey.Administration.WebApi
{
    public class AddClientModel
    {
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public string Contacts { get; set; }
        public string Email { get; set; }
        
        public Client GetClient() => new Client()
        {
            Name = Name,
            SecondName = SecondName,
            ThirdName = ThirdName,
            Contacts = Contacts,
            EmailAddress = Email
        };


    }
}
