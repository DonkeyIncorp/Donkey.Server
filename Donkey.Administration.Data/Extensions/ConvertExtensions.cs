using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectSystem;

namespace Donkey.Administration.Data.Extensions
{
    internal static class ConvertExtensions
    {
        public static DataModel.Order ToDbModel(this Order order)
        {
            Tag[] tags;

            if(order.Tags is Tag[])
            {
                tags = order?.Tags as Tag[];
            }
            else
            {
                tags = order?.Tags?.ToArray();
            }

            return new DataModel.Order()
            {
                Guid = order?.Guid,
                Status = order.Status,
                ArrivalTime = order.ArrivalTime,
                FinishTime = order?.FinishTime,
                Tags = tags ?? Array.Empty<Tag>(),
                Name = order?.Name,
                Description = order?.Description,
                LastUpdateTime = order?.LastUpdateTime,
                PercentCompleted = order?.PercentCompleted,
                Resources = order?.Resources
            };
        }
        public static Order ToObjectModel(this DataModel.Order order)
        {
            return new Order()
            {
                Status = order.Status,
                ArrivalTime = order.ArrivalTime,
                Client = order?.Client?.ToObjectModel(),
                Description = order?.Description,
                FinishTime = order?.FinishTime,
                Guid = order?.Guid,
                Tags = order?.Tags,
                LastUpdateAdminLogin = order?.LastUpdateAdmin?.Username,
                LastUpdateTime = order?.LastUpdateTime,
                PercentCompleted = order?.PercentCompleted,
                Name = order?.Name,
                Resources = order?.Resources
            };
        }

        public static DataModel.Client ToDbModel(this Client client)
        {
            return new DataModel.Client()
            {
                SecondName = client.SecondName,
                Email = client.EmailAddress,
                Name = client.Name,
                Contacts = client.Contacts,
                ThirdName = client.ThirdName,
                Id = (int)client.Id
            };
        }

        public static Client ToObjectModel(this DataModel.Client client)
        {
            return new Client()
            {
                SecondName = client.SecondName,
                Name = client.Name,
                EmailAddress = client.Email,
                Contacts = client.Contacts,
                ThirdName = client.ThirdName,
                Id = client.Id
            };
        }

    }
}
