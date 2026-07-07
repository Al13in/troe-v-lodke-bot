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
            // Пробуем разные варианты написания #reversed#
            var match = Regex.Match(input, @"#[Rr][Ee][Vv][Ee][Rr][Ss][Ee][Dd]#(.+)", RegexOptions.Singleline);
            if (match.Success)
            {
                string text = match.Groups[1].Value.TrimEnd('\r', '\n', ' ');
                return new string(text.Reverse().ToArray());
            }

            // Также пробуем найти паттерн reversed: или Reversed: 
            var match2 = Regex.Match(input, @"[Rr]eversed[:=\s]+(.+)", RegexOptions.Singleline);
            if (match2.Success)
            {
                string text = match2.Groups[1].Value.TrimEnd('\r', '\n', ' ');
                return new string(text.Reverse().ToArray());
            }

            // Фолбэк: переворачиваем всю строку
            string trimmed = input.Trim();
            return new string(trimmed.Reverse().ToArray());
        }
        catch { return ""; }
    }

    // steganography: первая строка — римская цифра N
    // остальные строки — текст, нужно вернуть N-е слово
    public static string SolveSteganography(string input)
    {
        try
        {
            // Нормализуем переносы строк
            string normalized = input.Replace("\r\n", "\n").Replace("\r", "\n");
            var lines = normalized.Split('\n');

            // Находим первую непустую строку как индекс
            int firstLineIdx = 0;
            while (firstLineIdx < lines.Length && string.IsNullOrWhiteSpace(lines[firstLineIdx]))
                firstLineIdx++;

            if (firstLineIdx >= lines.Length) return "";

            string firstLine = lines[firstLineIdx].Trim();
            int n = RomanToInt(firstLine.ToUpper());

            if (n <= 0) return "";

            // Собираем все слова из остальных строк
            var allWords = lines
                .Skip(firstLineIdx + 1)
                .SelectMany(line => line.Trim().Split(new char[] { ' ', '\t', ',', '.', ';', ':', '!', '?' }, StringSplitOptions.RemoveEmptyEntries))
                .ToArray();

            if (n >= 1 && n <= allWords.Length)
                return allWords[n - 1];

            return "";
        }
        catch { return ""; }
    }

    private static int RomanToInt(string s)
    {
        // Берём только буквы римского алфавита
        s = Regex.Replace(s, @"[^IVXLCDM]", "");
        if (string.IsNullOrEmpty(s)) return 0;

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
