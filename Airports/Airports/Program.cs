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
            var pattern = "^[0-9]{1,4},(\".*\",){5}([-0-9]{1,4}(\\.[0-9]{0,})?,){2}";

            using (var stream = new FileStream(@"Datas\airports.dat", FileMode.Open))
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!Regex.Match(line, pattern).Success)
                    {
                        logger.Info($"The next row (\"{line}\") is not match with the pattern.");
                        continue;
                    }
                }
            }
        }
    }
}
