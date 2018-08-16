using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ClearCache1C
{
    internal class DefaultValues
    {
        internal static readonly string pathAppDataRoaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        internal static readonly string pathAppDataLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        
        internal static readonly string postfixCache = Path.Combine("1c", "1cv8");

        internal static string GetCacheAppDataRoaming => Path.Combine(pathAppDataRoaming, postfixCache);
        internal static string GetCacheAppDataLocal => Path.Combine(pathAppDataLocal, postfixCache);

    }
}
