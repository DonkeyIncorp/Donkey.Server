using Donkey.Administration.Data.DataModel;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using NLog;
using System;

namespace Donkey.Administration.Data
{
    internal abstract class DataAccess
    {
        protected IDbContextFactory<DataContext> ContextFactory { get; }

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public DataAccess(IDbContextFactory<DataContext> contextFactory)
        {
            ContextFactory = contextFactory;
        }

        protected async Task<Client> GetDbClientAsync(string email, DataContext context)
        {
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            try
            {
                Client dbClient = await context.Clients.FirstOrDefaultAsync(x => x.Email == email).ConfigureAwait(false);
                   
                return dbClient;
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Не удалось получить информацию о клиенте {email ?? "null"}");
                throw;
            }
        }
    }
}
