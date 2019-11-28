using cbgb.Model;
using cbgb.Model.Enum;
using cbgb.Resources;
using cbgb.Utils;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections;
using System.Collections.Generic;

namespace cbgb.Sheet
{
    class SheetManager
    {
        private SheetsService service;
        private readonly string spreadSheetId = Resource.GetResorceString(EResources.spreadsheetid);

        public SheetManager()
        {
            service = new Service().SheetService;
        }

        public void UpdateSheet()
        {
            FetchSheetData();
            var sheetData = Resource.SheetData;
            var guildBank = Resource.GuildBank;

            CompareSheetWithBank(sheetData, guildBank);
            var ranges = CreateRanges(sheetData, guildBank);
            
            var valList = CreateValueList(ranges);
            var body = new BatchUpdateValuesRequest()
            {
                ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW.ToString(),
                Data = valList
            };
            var result = service.Spreadsheets.Values.BatchUpdate(body, spreadSheetId).Execute();
        }

        private List<ValueRange> CreateValueList(Dictionary<string, Dictionary<string, Range>> ranges)
        {
            var valList = new List<ValueRange>();
            var tabs = Resource.SheetTabs;

            foreach(var tab in tabs)
            {
                if (ranges.ContainsKey(tab))
                {
                    foreach(var range in ranges[tab])
                    {
                        valList.Add(new ValueRange
                        {
                            MajorDimension = "COLUMNS",
                            Range = $"{tab}!{range.Value.Column}{range.Value.MinRow}:{range.Value.Column}{range.Value.MaxRow}",
                            Values = ValuesSorted(range.Value.Values)
                        });
                    }
                }
            }
            return valList;
        }

        private static List<IList<object>> ValuesSorted(List<SheetItem> values)
        {
            var sorter = new Sorter();
            values.Sort(sorter);
            var list = new List<object>();
            foreach (var value in values)
            {
                if (IsImport(value.Sheet) && (value.Column == "H"))
                    list.Add(value.Webaddress);//$"=HYPERLINK(\"{value.Webaddress}\"; \"{value.Name}\")");
                else
                    list.Add(value.Quantity);
            }
            return new List<IList<object>>() { list };
            
        }
        private static Dictionary<string, Dictionary<string, Range>> CreateRanges(List<SheetItem> sheetData, List<Item> guildBank)
        {
            var ranges = new Dictionary<string, Dictionary<string, Range>>();
            foreach (var item in guildBank)
            {
                var sheetItem = sheetData.Find(si => si.Id == item.Id);
                if (ranges.ContainsKey(item.Sheet))
                {
                    if (IsImport(item.Sheet))
                    {
                        UpdateImportRange(ranges, sheetData, item);
                    } else
                    {
                        if (ranges[item.Sheet].ContainsKey(sheetItem.Column))
                            UpdateRange(ranges[item.Sheet][sheetItem.Column], sheetItem, item);
                        else
                            ranges[item.Sheet].Add(sheetItem.Column, NewRange(sheetItem, item));
                    }
                }
                else
                {
                    ranges.Add(item.Sheet, new Dictionary<string, Range>());
                    if (IsImport(item.Sheet))
                        NewImportRange(ranges, sheetData, item);
                    else
                        ranges[item.Sheet].Add(sheetItem.Column, NewRange(sheetItem, item));
                }
            }
            return ranges;
        }

        private static void UpdateImportRange(Dictionary<string, Dictionary<string, Range>> ranges, List<SheetItem> sheetData, Item item)
        {
            var sheetItems = sheetData.FindAll(i => i.Id == item.Id);
            foreach(var sheetItem in sheetItems)
            {
                UpdateRange(ranges[item.Sheet][sheetItem.Column], sheetItem, item);
            }
        }

        private static void NewImportRange(Dictionary<string, Dictionary<string, Range>> ranges, List<SheetItem> sheetData, Item item)
        {
            var sheetItems = sheetData.FindAll(i => i.Id == item.Id);
            foreach (var sheetItem in sheetItems)
            {
                ranges[item.Sheet].Add(sheetItem.Column, NewRange(sheetItem, item));
            }
        }

        private static void UpdateRange(Range range, SheetItem sheetItem, Item item)
        {
            if (range.MaxRow < sheetItem.Row)
                range.MaxRow = sheetItem.Row;
            if (range.MinRow > sheetItem.Row)
                range.MinRow = sheetItem.Row;
            range.Values.Add(sheetItem);
        }

        private static Range NewRange(SheetItem sheetItem, Item item)
        {
            return new Range
            {
                Column = sheetItem.Column,
                MaxRow = sheetItem.Row,
                MinRow = sheetItem.Row,
                Values = new List<SheetItem> { sheetItem }
            };
        }

        private void CompareSheetWithBank(List<SheetItem> sheetData, List<Item> guildBank)
        {
            var rowCounter = 1;
            foreach (var item in guildBank)
            {
                var sheetItem = sheetData.Find(si => si.Id == item.Id);
                if (sheetItem != null)
                {
                    item.IsInSheet = true;
                    item.Sheet = sheetItem.Sheet;
                    sheetItem.Quantity = item.Quantity;
                } else
                {
                    AddToImportSheet(sheetData, rowCounter, item);
                    rowCounter++;
                }
            }
        }

        private static void AddToImportSheet(List<SheetItem> sheetData, int rowCounter, Item item)
        {
            item.IsInSheet = false;
            item.Sheet = "Import";
            sheetData.Add(new SheetItem
            {
                Id = item.Id,
                Name = item.Name,
                Column = "I",
                Webaddress = Util.WowHeadLink(item.Name, item.Id),
                Row = rowCounter,
                Sheet = item.Sheet,
                Quantity = item.Quantity
            });

            sheetData.Add(new SheetItem
            {
                Id = item.Id,
                Name = item.Name,
                Column = "H",
                Webaddress = Util.WowHeadLink(item.Name, item.Id),
                Row = rowCounter,
                Sheet = item.Sheet
            });
        }

        public void FetchSheetData()
        {
            var sheetData = new List<SheetItem>();
            foreach (var value in Resource.SheetTabs)
            {
                var range = value;

                var request = service.Spreadsheets.Values.Get(spreadSheetId, range);
                request.ValueRenderOption = SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum.FORMULA;
                var response = request.Execute();

                var includeNextCol = false;
                SheetItem sheetItem = null;
                if (response.Values != null)
                {
                    for (var row = 0; row < response.Values.Count; row++)
                    {
                        for (var col = 0; col < response.Values[row].Count; col++)
                        {
                            if (includeNextCol)
                            {
                                includeNextCol = false;
                                var qty = 0;
                                if (Int32.TryParse(response.Values[row][col].ToString(), out qty))
                                    sheetItem.Quantity = qty;
                                sheetItem.Column = Util.ConverCol(col);
                            }
                            if ((response.Values[row][col].ToString().Contains("HYPERLINK")) ||
                                (response.Values[row][col].ToString().Contains("IMAGE")))
                            {
                                sheetItem = new SheetItem
                                {
                                    Id = Util.GetIdFromFormula(response.Values[row][col].ToString()),
                                    Row = row + 1,
                                    Sheet = range,
                                    Webaddress = Util.GetHyperLinkFromFormula(response.Values[row][col].ToString()),
                                    ImageAddress = Util.GetImageLinkFromFormula(response.Values[row][col].ToString())
                                };

                                includeNextCol = true;
                                sheetData.Add(sheetItem);
                            }
                        }
                    }
                }
                if (sheetData.Count > 0)
                    JsonManager.Save(Resource.GetResorceString(EResources.sheetdata), sheetData);
            }
        }

        private static bool IsImport(string tab)
        {
            return tab == "Import";
        }
    }
}
