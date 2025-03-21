using System.Text.RegularExpressions;

namespace MovieBot.Helpers
{
    public static class ParserHelper
    {
        public static Dictionary<string, string> DecodeStreams(string data)
        {
            const string salt = "@#!^$";
            List<int> iterations = [2, 3];
            var saltCombos = iterations
                .SelectMany(it => Product(salt, it), (it, s) => System.Text.Encoding.UTF8.GetBytes(s))
                .Select(Convert.ToBase64String)
                .ToList();

            saltCombos.Add("#h");
            saltCombos.Add("//_//");

            data = saltCombos.Aggregate(data, (current, combo) => current.Replace(combo, ""));

            var dataBytes = Convert.FromBase64String(data);
            var result = System.Text.Encoding.UTF8.GetString(dataBytes);

            return result.Split([','])
                .ToDictionary(
                    key => Regex.Match(key, @"(?<=\[).*(?=\])").Value, 
                    value => Regex.Match(value, @"(?<=or\s).*").Value);
        }

        private static List<string> Product(string income, int times)
        {
            var res = new List<string> { string.Empty };

            for (var i = 0; i < times; i++)
            {
                var tmp = res
                    .SelectMany(s => income, (s, c) => s + c)
                    .ToList();

                res = tmp;
            }

            return res;
        }
    }
}
