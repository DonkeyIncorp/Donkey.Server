using System;
using System.Collections.Generic;
using System.Text;

namespace Donkey.Administration.WebApi
{
    public interface IHashEncoder
    {
        string GetHash(string data);
    }
}
