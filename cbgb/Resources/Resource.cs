using cbgb.Model;
using cbgb.Model.Enum;
using cbgb.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace cbgb.Resources
{
    class Resource
    {
        private static Dictionary<EResources,string> resources;
        readonly static string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly()?.Location);
        readonly static string resourcePath;

        static Resource()
        {
            resources = JsonManager.Load<Dictionary<EResources, string>>(basePath + @"\Resources\resources.json");
            resourcePath = basePath + resources[EResources.basePath];
        }

        public static string SpreadSheetId { get; }

        public static List<SheetItem> SheetData
        {
            get
            {
                return JsonManager.Load<List<SheetItem>>(GetResorceString(EResources.sheetdata));
            }
        }

        public static Dictionary<int,Item> ItemDb {
            get
            {
                return JsonManager.Load<Dictionary<int, Item>>(GetResorceString(EResources.itemDb));
            }
        }

        public static List<Item> GuildBank
        {
            get
            {
                return JsonManager.Load<List<Item>>(GetResorceString(EResources.guildbank));
            }
        }

        public static List<string> SheetTabs {
            get
            {
                return TabsInEResource();
            }
        }

        public static string GetResorceString(EResources path)
        {
            switch (path)
            {
                case EResources.basePath:
                    return resourcePath;
                case EResources.itemDb:
                    return resourcePath + resources[path];
                case EResources.guildbank:
                    return resourcePath + resources[path];
                case EResources.cred:
                    return resourcePath + resources[path];
                case EResources.sheetdata:
                    return resourcePath + resources[path];
                default:
                    return resources[path];
            }
        }

        private static List<string> TabsInEResource()
        {
            
            var tabs = new List<string>();
            foreach(var value in Enum.GetValues(typeof(EResources)))
            {
                if (value.ToString().Contains("tab"))
                {
                    var val = (EResources)value;
                    tabs.Add(GetResorceString(val));
                }
            }
            return tabs;
        }
    }
}
