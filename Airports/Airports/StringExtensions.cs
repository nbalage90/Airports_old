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

        public static string ToShortString(this string input)
        {
            if (!string.IsNullOrEmpty(input) && input.Substring(0, 1) == "\"")
            {
                input = input.Substring(1);
            }

            if (!string.IsNullOrEmpty(input) && input.Substring(input.Length - 1) == "\"")
            {
                input = input.Substring(0, input.Length - 1);
            }

            return input;
        }
    }
}
