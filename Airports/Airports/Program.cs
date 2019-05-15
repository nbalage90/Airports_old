using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Airports
{
    class Program
    {
        static Logger logger;

        static void Main(string[] args)
        {
            Initialize();
            TryTransormDatas();
        }

        static void Initialize()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        static bool IsOwnDataFileExsists()
        {
            //TODO: IsOwnDataFileExsists
            return false;
        }

        static void TryTransormDatas()
        {
            var pattern = "[0-9]{4},(\"[A-Za-z]*\",){5}([0-9]{1,2}\\.[0-9]{6},){2}";

            
        }
    }
}
