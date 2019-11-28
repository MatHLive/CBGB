using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace cbgb.Utils
{
    class PathUtil
    {
        public static bool IsValidPath(string path)
        {
            try
            {
                Path.GetFullPath(path);
                return File.Exists(path);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static string HandleCopiedPath(string path)
        {
            if (path.IndexOf("\"") == 0)
                path = path.Substring(1, path.Length - 2);
            return path;
        }
    }
}
