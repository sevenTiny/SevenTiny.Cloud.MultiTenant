﻿using MongoDB.Bson;
using MongoDB.Driver;
using SevenTiny.Bantina.Bankinate;
using SevenTiny.Cloud.Infrastructure.Configs;

namespace SevenTiny.Cloud.MultiTenantPlatform.UI.DataAccess
{
    public class MultiTenantDataDbContext : MongoDbContext<MultiTenantDataDbContext>
    {
        public MultiTenantDataDbContext() : base(ConnectionStringsConfig.Instance.mongodb39911)
        {
        }
    }
}
