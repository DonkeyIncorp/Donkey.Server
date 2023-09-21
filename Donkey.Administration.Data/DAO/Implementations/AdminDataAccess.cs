using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NLog;
using Donkey.Administration.Environment;

namespace Donkey.Administration.Data
{
    internal class AdminDataAccess : DataAccess, IAdminDataAccess
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public AdminDataAccess(IDbContextFactory<DataContext> dbContextFactory) : base(dbContextFactory)
        {

        }
        public async Task AddAdminAsync(string username, string passwordHash)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    throw new ArgumentNullException(nameof(username));
                }
                if (string.IsNullOrEmpty(passwordHash))
                {
                    throw new ArgumentNullException(nameof(passwordHash));
                }

                using (var context = ContextFactory.CreateDbContext())
                {
                    bool adminExists = await context.Admins.AnyAsync(x => x.Username == username);

                    if (adminExists)
                    {
                        throw new ClientExistsException(username);
                    }

                    await context.Admins.AddAsync(new DataModel.Admin()
                    {
                        Username = username,
                        PasswordHash = passwordHash
                    });

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалось добавить администратора {username ?? "[username не указан]"}");
                throw;
            }
            
        }

        public async Task<bool> ValidateAuthDataAsync(string username, string passwordHash)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    throw new ArgumentNullException(nameof(username));
                }

                if (string.IsNullOrEmpty(passwordHash))
                {
                    throw new ArgumentNullException(nameof(passwordHash));
                }

                using(var context = ContextFactory.CreateDbContext())
                {
                    var admin = await context.Admins.FirstOrDefaultAsync(x => x.Username == username);

                    if(admin == null)
                    {
                        throw new ClientNotFoundException(username);
                    }

                    return admin.PasswordHash == passwordHash;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалось провести валидацию администратора {username ?? "[username не указан]"}");
                throw;
            }
        }
    }
}
