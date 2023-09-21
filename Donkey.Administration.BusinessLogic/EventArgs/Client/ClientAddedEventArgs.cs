using ObjectSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donkey.Administration.BusinessLogic
{
    public class ClientAddedEventArgs : EventArgs
    {
        public Client AddedClient { get; }

        public ClientAddedEventArgs(Client addedClient)
        {
            AddedClient = addedClient;
        }
    }
}
