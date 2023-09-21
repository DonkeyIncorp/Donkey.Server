using ObjectSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donkey.Administration.BusinessLogic
{
    public class ClientRemovedEventArgs : EventArgs
    {
        public Client RemovedClient { get; }

        public ClientRemovedEventArgs(Client removedClient)
        {
            RemovedClient = removedClient;
        }
    }
}
