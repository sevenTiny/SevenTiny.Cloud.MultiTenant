﻿using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using SevenTiny.Bantina.Caching;
using SevenTiny.Cloud.MultiTenantPlatform.Domain.CloudEntity;
using SevenTiny.Cloud.MultiTenantPlatform.Domain.Enum;
using SevenTiny.Cloud.MultiTenantPlatform.Domain.ServiceContract;
using SevenTiny.Cloud.MultiTenantPlatform.Infrastructure.Caching;
using SevenTiny.Cloud.MultiTenantPlatform.TriggerScriptEngine.Models;
using SevenTiny.Cloud.MultiTenantPlatform.TriggerScriptEngine.ServiceContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SevenTiny.Cloud.MultiTenantPlatform.TriggerScriptEngine.Service
{
    public class TriggerScriptEngineService : ITriggerScriptEngineService
    {
        public TriggerScriptEngineService(ITriggerScriptService _triggerScriptService)
        {
            triggerScriptService = _triggerScriptService;
        }

        readonly ITriggerScriptService triggerScriptService;

        /// <summary>
        /// using 分隔符（脚本中必须存在该分割符）
        /// </summary>
        private readonly string USING_SEPARATOR = "//end using";

        public TableListComponent TableListAfter(int metaObjectId, string operateCode, TableListComponent tableListComponent)
        {
            var triggerScripts = triggerScriptService.GetTriggerScriptsUnDeletedByMetaObjectIdAndScriptType(metaObjectId, (int)ScriptType.TableList, (int)TriggerPoint.After);
            if (triggerScripts != null && triggerScripts.Any())
            {
                foreach (var item in triggerScripts)
                {
                    tableListComponent = ExecuteTableListAfter(operateCode, tableListComponent, item.Script);
                }
            }
            return tableListComponent;
        }

        private TableListComponent ExecuteTableListAfter(string operateCode, TableListComponent tableListComponent, string triggerScript)
        {
            var hashKey = $"{operateCode}_{triggerScript.GetHashCode()}".GetHashCode();
            var triggerScriptInCache = TriggerScriptCache.GetSet(hashKey, () =>
            {
                //检查标识是否存在
                if (!triggerScript.Contains(USING_SEPARATOR))
                    throw new KeyNotFoundException($"{USING_SEPARATOR} not found in trigger script");

                string[] scriptArray = Regex.Split(triggerScript, USING_SEPARATOR, RegexOptions.IgnoreCase);

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("using SevenTiny.Cloud.MultiTenantPlatform.Domain.CloudEntity;");

                //append using
                stringBuilder.Append(scriptArray[0]);

                stringBuilder.Append("public class TriggerScript_TableListComponent");
                stringBuilder.Append("{");
                stringBuilder.Append(scriptArray[1]);
                stringBuilder.Append("}");

                stringBuilder.Append("return new TriggerScript_TableListComponent().TableListAfter(operateCode,tableListComponent);");

                var script = CSharpScript.Create<TableListComponent>(stringBuilder.ToString(),
                    ScriptOptions.Default
                    //引用dll
                    .AddReferences("System")
                    .AddReferences("System.Collections.Generic")
                    .AddReferences("System.Linq")
                    .AddReferences("System.Text")
                    .AddReferences("SevenTiny.Cloud.MultiTenantPlatform.Domain"),
                    globalsType: typeof(TableListArg)
                    );

                script.Compile();

                return script;
            });

            var result = triggerScriptInCache.RunAsync(globals: new TableListArg { operateCode = operateCode, tableListComponent = tableListComponent }).Result.ReturnValue;

            return result;
        }
    }
}
