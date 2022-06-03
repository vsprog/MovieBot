using System.Text.RegularExpressions;

namespace MovieBot.Infractructure
{
    public static class ParserHelper
    {
        public static Dictionary<string, string> DecodeStreams(string data)
        {
            string salt = "@#!^$";
            int[] iterations = new[] { 2, 3 };
            var saltCombos = new List<string>();

            foreach (int it in iterations)
            {
                foreach (var s in Product(salt, it))
                {
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(s);
                    saltCombos.Add(Convert.ToBase64String(plainTextBytes));
                }
            }

            saltCombos.Add("#h");
            saltCombos.Add("//_//");

            foreach (string combo in saltCombos)
            {
                data = data.Replace(combo, "");
            }

            byte[] dataBytes = Convert.FromBase64String(data);
            string result = System.Text.Encoding.UTF8.GetString(dataBytes);

            return result.Split(new[] { ',' })
                .ToDictionary(
                    key => Regex.Match(key, @"(?<=\[).*(?=\])").Value, 
                    value => Regex.Match(value, @"(?<=or\s).*").Value);
        }

        private static List<string> Product(string income, int times)
        {
            var res = new List<string>() { string.Empty };

            for (int i = 0; i < times; i++)
            {
                var tmp = new List<string>();

                foreach (string s in res)
                {
                    foreach (char c in income) tmp.Add(s + c);
                }

                res = tmp;
            }

            return res;
        }
    }
}
