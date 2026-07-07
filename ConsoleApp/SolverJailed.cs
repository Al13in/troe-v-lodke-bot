using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp;

public static class SolverJailed
{
    // Задача cypher: вопрос содержит #reversed#текст#reversed#
    // Нужно извлечь текст между маркерами и вернуть его в обратном порядке
    public static string SolveCypher(string input)
    {
        try
        {
            var match = Regex.Match(input, @"#reversed#(.+?)#reversed#");
            if (match.Success)
            {
                string text = match.Groups[1].Value;
                return new string(text.Reverse().ToArray());
            }
            // Если маркер только один — разворачиваем всё что после него
            var simpleMatch = Regex.Match(input, @"#reversed#(.+)");
            if (simpleMatch.Success)
            {
                string text = simpleMatch.Groups[1].Value.Trim();
                return new string(text.Reverse().ToArray());
            }
            return new string(input.Reverse().ToArray());
        }
        catch { return ""; }
    }

    // Задача steganography: вопрос начинается со строк с римскими цифрами
    // Каждая строка — римская цифра = номер буквы в алфавите (I=A, II=B, ...)
    public static string SolveSteganography(string input)
    {
        try
        {
            var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                string trimmed = line.Trim();
                // Пропускаем строки, которые не являются римскими цифрами
                if (!Regex.IsMatch(trimmed, @"^[IVXLCDMivxlcdm]+$"))
                    break;
                int value = RomanToInt(trimmed.ToUpper());
                if (value >= 1 && value <= 26)
                    sb.Append((char)('A' + value - 1));
            }
            return sb.ToString();
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
            int cur = map[s[i]];
            int next = (i + 1 < s.Length) ? map[s[i + 1]] : 0;
            if (cur < next)
                result -= cur;
            else
                result += cur;
        }
        return result;
    }
}
