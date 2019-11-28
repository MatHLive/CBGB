using System;
using System.Collections.Generic;
using System.Text;

namespace cbgb.Utils
{
    class Util
    {
        public static string ConverCol(int col)
        {
            var values = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return values[col]+"";
        }

        public static int GetIdFromFormula(string formula)
        {
            var idInt = -1;
            var id = "";
            if (formula.Contains("HYPERLINK"))
            {
                var startIndex = formula.IndexOf("item=");
                if (startIndex < 0)
                    startIndex = formula.IndexOf("spell=");
                //TODO: handel startindex < 0
                if (startIndex < 0)
                    return idInt;
                id = formula.Substring(startIndex);
                id = id.Substring(5, id.IndexOf("/")-5);
            }
            if ((id != "") && Int32.TryParse(id, out idInt))
                return idInt;
            return idInt;
        }

        public static string GetHyperLinkFromFormula(string formula)
        {
            if(formula != "")
                formula = formula.Substring(formula.IndexOf("\"")+1, formula.IndexOf(";") - formula.IndexOf("\"")-2);
            return formula;
        }

        public static string GetImageLinkFromFormula(string formula)
        {
            if (formula != "" && formula.IndexOf("IMAGE(\"") > 0)
            {
                formula = formula.Substring(formula.IndexOf("IMAGE(\"") + 7);
                formula = formula.Substring(0, formula.IndexOf("\""));
            }
            return formula;
        }

        public static string WowHeadLink(string name, int id)
        {
            if (name.Contains(" "))
            {
                var tempName = name.Replace(" ", "-").ToLower();
                return $"https://classic.wowhead.com/item={id}/{tempName}";
            }
            return $"https://classic.wowhead.com/item={id}/{name.ToLower()}";
        }
    }
}
