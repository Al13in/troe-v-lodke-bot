using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp;

public static class SolverJailed
{
    // cypher: вопрос содержит #reversed#текст
    // нужно перевернуть всё что идёт после #reversed#
    public static string SolveCypher(string input)
    {
        try
        {
            var match = Regex.Match(input, @"#reversed#(.+)", RegexOptions.Singleline);
            if (match.Success)
            {
                string text = match.Groups[1].Value.TrimEnd('\r', '\n', ' ');
                return new string(text.Reverse().ToArray());
            }
            // Фолбэк: просто переворачиваем всю строку
            return new string(input.Trim().Reverse().ToArray());
        }
        catch { return ""; }
    }

    // steganography: первая строка — римская цифра N
    // остальные строки — текст, нужно вернуть N-е слово
    public static string SolveSteganography(string input)
    {
        try
        {
            // Нормализуем переносы строк (\r\n -> \n)
            string normalized = input.Replace("\r\n", "\n").Replace("\r", "\n");
            var lines = normalized.Split('\n');

            string firstLine = lines[0].Trim();
            int n = RomanToInt(firstLine.ToUpper());

            // Собираем все слова из остальных строк
            var allWords = lines
                .Skip(1)
                .SelectMany(line => line.Trim().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries))
                .ToArray();

            if (n >= 1 && n <= allWords.Length)
                return allWords[n - 1];

            return "";
        }
        catch { return ""; }
    }

    private static int RomanToInt(string s)
    {
        // Берём только буквы (убираем лишние символы если есть)
        s = Regex.Replace(s, @"[^IVXLCDM]", "");
        var map = new System.Collections.Generic.Dictionary<char, int>
        {
            { 'I', 1 }, { 'V', 5 }, { 'X', 10 }, { 'L', 50 },
            { 'C', 100 }, { 'D', 500 }, { 'M', 1000 }
        };
        int result = 0;
        for (int i = 0; i < s.Length; i++)
        {
            if (!map.ContainsKey(s[i])) continue;
            int cur = map[s[i]];
            int next = (i + 1 < s.Length && map.ContainsKey(s[i + 1])) ? map[s[i + 1]] : 0;
            result += cur < next ? -cur : cur;
        }
        return result;
    }
}
