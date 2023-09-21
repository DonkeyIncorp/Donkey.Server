using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Donkey.Administration.Data
{
    public interface IAdminDataAccess
    {
        Task<bool> ValidateAuthDataAsync(string username, string passwordHash);
        Task AddAdminAsync(string username, string passwordHash);
    }
}
