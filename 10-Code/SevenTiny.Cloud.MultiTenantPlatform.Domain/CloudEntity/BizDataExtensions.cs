﻿using MongoDB.Bson;
using SevenTiny.Cloud.MultiTenantPlatform.Domain.UIMetaData;
using System.Collections.Generic;

namespace SevenTiny.Cloud.MultiTenantPlatform.Domain.CloudEntity
{
    /// <summary>
    /// 业务数据转化为字典
    /// </summary>
    public static class BizDataExtensions
    {
        public static Dictionary<string, FieldBizData> ToBizDataDictionary(this BsonDocument bsonElements)
        {
            Dictionary<string, FieldBizData> keyValuePairs = new Dictionary<string, FieldBizData>();
            foreach (var item in bsonElements)
            {
                keyValuePairs.Add(item.Name, new FieldBizData
                {
                    Name = item.Name,
                    Text = item.Value?.ToString(),
                    Value = item.Value
                });
            }
            return keyValuePairs;
        }
    }
}
