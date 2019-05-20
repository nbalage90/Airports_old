using System;
using System.Collections.Generic;
using System.Text;

namespace Airports
{
    public static class StringExtensions
    {
        public static string[] AirportSplit(this string input, char separator)
        {
            List<string> tokens = new List<string>();
            StringBuilder sb = new StringBuilder();
            bool closedString = true;

            foreach (char c in input)
            {
                if (c == '"')
                {
                    closedString = !closedString;
                }

                if (c == separator && closedString)
                {
                    tokens.Add(sb.ToString().Trim());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }

            return tokens.ToArray();
        }
    }
}
