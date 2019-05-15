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
            var pattern = "^[0-9]{1,4},(\".*\",){5}([-0-9]{1,3}(\\.[0-9]{0,6})?,){2}";

            //var line = "1,\"Goroka\",\"Goroka\",\"Papua New Guinea\",\"GKA\",\"AYGA\",-6.081689,145.391881,5282,10,\"U\"";
            //var line2 = "1489,\"Ferihegy\",\"Budapest\",\"Hungary\",\"BUD\",\"LHBP\",47.436933,19.255592,495,1,\"E\"";
            //Console.WriteLine(Regex.Match(line, pattern).Success);
            //Console.WriteLine(Regex.Match(line2, pattern).Success);

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
