using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp;

public static class SolverJailed
{
    // cypher: вопрос #reversed#текст
    // всё что идёт после #reversed# — перевернуть, ответ включает # в начале
    public static string SolveCypher(string input)
    {
        try
        {
            var match = Regex.Match(input, @"#reversed#(.+)", RegexOptions.Singleline);
            if (match.Success)
            {
                string text = match.Groups[1].Value.TrimEnd();
                return new string(text.Reverse().ToArray());
            }
            return new string(input.Reverse().ToArray());
        }
        catch { return ""; }
    }

    // steganography: первая строка — римская цифра N
    // остальные строки — текст, нужно вернуть N-е слово из всего текста
    public static string SolveSteganography(string input)
    {
        try
        {
            var lines = input.Split('\n');
            string firstLine = lines[0].Trim();
            int n = RomanToInt(firstLine.ToUpper());

            // Собираем все слова из остальных строк
            var allWords = lines
                .Skip(1)
                .SelectMany(line => line.Trim().Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries))
                .ToArray();

            if (n >= 1 && n <= allWords.Length)
                return allWords[n - 1];
            return "";
        }
        catch { return ""; }
    }

    private static int RomanToInt(string s)
    {
        var map = new System.Collections.Generic.Dictionary<char, int>
        {
            {'I', 1}, {'V', 5}, {'X', 10}, {'L', 50},
            {'C', 100}, {'D', 500}, {'M', 1000}
        };
        int result = 0;
        for (int i = 0; i < s.Length; i++)
        {
            int cur = map.ContainsKey(s[i]) ? map[s[i]] : 0;
            int next = (i + 1 < s.Length && map.ContainsKey(s[i + 1])) ? map[s[i + 1]] : 0;
            result += cur < next ? -cur : cur;
        }
        return result;
    }
}
