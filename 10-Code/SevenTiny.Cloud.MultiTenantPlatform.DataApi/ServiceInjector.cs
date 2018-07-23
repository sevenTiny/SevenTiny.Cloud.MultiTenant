﻿using Microsoft.Extensions.DependencyInjection;
using SevenTiny.Cloud.MultiTenantPlatform.Application.Service;
using SevenTiny.Cloud.MultiTenantPlatform.Application.ServiceContract;
using SevenTiny.Cloud.MultiTenantPlatform.DomainModel.Repository;
using SevenTiny.Cloud.MultiTenantPlatform.DomainModel.RepositoryContract;

namespace SevenTiny.Cloud.MultiTenantPlatform.DataApi
{
    /// <summary>
    /// 依赖注入器
    /// </summary>
    public static class ServiceInjector
    {
        //使用.netcore自带的DI
        public static IServiceCollection NetCoreInject(IServiceCollection services)
        {
            //repository
            services.AddScoped<IMetaObjectRepository, MetaObjectRepository>();
            services.AddScoped<IMetaFieldRepository, MetaFieldRepository>();
            services.AddScoped<IInterfaceAggregationRepository, InterfaceAggregationRepository>();

            //service
            services.AddScoped<IObjectDataService, ObjectDataService>();
            services.AddScoped<IAggregationConditionService, AggregationConditionService>();

            return services; 
        }

        //使用Autofac
        public static IServiceCollection AutofacInject(IServiceCollection services)
        {
            return services;
        }
    }
}