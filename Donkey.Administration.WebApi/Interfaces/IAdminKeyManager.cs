using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Donkey.Administration.WebApi
{
    public interface IAdminKeyManager : IKeyManager
    {
        Task<string> GetKeyAsync(string username, string passwordHash);
        Task<string> GetLoginAsync(string key);
    }
}
