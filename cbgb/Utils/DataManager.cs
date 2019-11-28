using cbgb.Model;
using cbgb.Model.Enum;
using cbgb.Resources;
using System;
using System.Collections.Generic;
using System.IO;

namespace cbgb.Utils
{
    class DataManager
    {
        public static void UpdateGuildBank(List<Item> items)
        {
            var today = DateTime.Today;
            var guildBank = Resource.GuildBank;
            if (guildBank == null)
                guildBank = new List<Item>();

            foreach (var item in items)
            {
                var gItem = guildBank.Find(i => i.Id == item.Id);
                if (gItem != null)
                {
                    if (DateTime.Compare(gItem.LastUpdate, today) < 0)
                    {
                        gItem.Quantity = 0;
                        gItem.LastUpdate = today;
                    }
                    gItem.Quantity += item.Quantity;
                } else
                {
                    var tempItem = Resource.ItemDb[item.Id];
                    tempItem.LastUpdate = today;
                    tempItem.Quantity = item.Quantity;
                    guildBank.Add(tempItem);
                }
            }
            JsonManager.Save(Resource.GetResorceString(EResources.guildbank), guildBank);
        }

        public static List<Item> ReadImportFile(string path)
        {
            path = PathUtil.HandleCopiedPath(path);

            var items = new List<Item>();
            var moneyOnNextLine = false;
            foreach (var line in File.ReadAllLines(path))
            {
                if (line != string.Empty)
                {
                    var lineData = line.Split("\t");

                    if (moneyOnNextLine)
                    {
                        moneyOnNextLine = false;
                        MoneyLine(line, items);
                    }

                    if (lineData.Length == 2)
                    {
                        int data0, data1;
                        if ((Int32.TryParse(lineData[0], out data0)) && (Int32.TryParse(lineData[1], out data1)))
                            items.Add(new Item(data0, data1));
                    }
                    else
                        moneyOnNextLine = IsNextMoneyLine(line);
                }
            }
            return items;
        }

        private static Item CreateMoneyItem(string currencyType,int amount)
        {
            switch (currencyType.ToLower())
            {
                case "g":
                    return new Item
                    {
                        Id = 1000000001,
                        Name = "Gold",
                        Quantity = amount
                    };
                case "s":
                    return new Item
                    {
                        Id = 1000000002,
                        Name = "Silver",
                        Quantity = amount
                    };
                case "c":
                    return new Item
                    {
                        Id = 1000000003,
                        Name = "Copper",
                        Quantity = amount
                    };
                default:
                    return null;
            }
        }

        private static bool IsNextMoneyLine(string line)
        {
            DateTime date;
            var line2 = "";
            if (line.Length == 24)
            {
                line2 = line.Substring(0, 10);
                line2 += line.Substring(19, 5);
            }
            return DateTime.TryParse(line2, out date);
        }

        private static void MoneyLine(string line, List<Item> items)
        {
            var currencies = line.Split(" ");
            if (currencies.Length == 3)
            {
                foreach (var money in currencies)
                {
                    var money2 = money;
                    money2 = money2.Trim();
                    var amount = 0;
                    Item currency = null;
                    if (Int32.TryParse(money2.Substring(0, money2.Length - 1), out amount))
                        currency = CreateMoneyItem(money2.Substring(money2.Length - 1), amount);
                    if (currency != null)
                        items.Add(currency);
                }
            }
        }

        private static void RestoreItemDbFromRawData()
        {
            var rawDataPath = @"D:\Visual Studio\Projects\cbgb\cbgb\cbgb\bin\Debug\netcoreapp2.0\Resources\Bu\rawData.json";
            var rawDataJson = JsonManager.Load<Dictionary<int, string>>(rawDataPath);

            var itemDb = new Dictionary<int, Item>();
            foreach (var item in rawDataJson)
            {
                itemDb.Add(item.Key, new Item
                {
                    Id = item.Key,
                    Name = item.Value,
                    Webaddress = Util.WowHeadLink(item.Value, item.Key)
                });
            }

            JsonManager.Save(Resource.GetResorceString(EResources.itemDb), itemDb);
        }
    }

}
