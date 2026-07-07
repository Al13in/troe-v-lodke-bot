using System;
using System.Text.RegularExpressions;

namespace ConsoleApp;

public static class SolverOlesia
{
    public static string SolveSimpleMath(string input)
    {
        try
        {
            string clean = input.Replace("=", "").Trim();
            var match = Regex.Match(clean, @"(-?\d+)\s*([\+\-\*\/])\s*(-?\d+)");
            if (!match.Success) return "0";
            long num1 = long.Parse(match.Groups[1].Value);
            string op = match.Groups[2].Value;
            long num2 = long.Parse(match.Groups[3].Value);
            return op switch
            {
                "+" => (num1 + num2).ToString(),
                "-" => (num1 - num2).ToString(),
                "*" => (num1 * num2).ToString(),
                "/" => (num1 / num2).ToString(),
                _ => "0"
            };
        }
        catch { return "0"; }
    }

    public static string SolvePolynomial(string input)
    {
        try
        {
            string s = input.Replace(" ", "");
            double a = 0, b = 0, c = 0;
            var aMatch = Regex.Match(s, @"([+-]?\d*\.?\d*)\*?x\^2");
            if (aMatch.Success)
                a = string.IsNullOrEmpty(aMatch.Groups[1].Value) || aMatch.Groups[1].Value == "+" ? 1
                    : (aMatch.Groups[1].Value == "-" ? -1 : double.Parse(aMatch.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture));
            var bMatch = Regex.Match(s, @"([+-]?\d*\.?\d*)\*?x(?!\^2)");
            if (bMatch.Success)
                b = string.IsNullOrEmpty(bMatch.Groups[1].Value) || bMatch.Groups[1].Value == "+" ? 1
                    : (bMatch.Groups[1].Value == "-" ? -1 : double.Parse(bMatch.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture));
            var cMatch = Regex.Match(s, @"([+-]?\d+\.?\d*)$");
            if (cMatch.Success)
                c = double.Parse(cMatch.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);

            // Линейное уравнение
            if (a == 0 && b != 0)
                return Math.Round(-c / b, 3).ToString(System.Globalization.CultureInfo.InvariantCulture);

            double d = b * b - 4 * a * c;
            if (d < 0) return "no roots";

            double x1 = (-b + Math.Sqrt(d)) / (2 * a);
            double x2 = (-b - Math.Sqrt(d)) / (2 * a);

            if (Math.Abs(d) < 1e-9)
            {
                // Один корень
                return Math.Round(x1, 3).ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            // Два корня — возвращаем больший первым
            double r1 = Math.Round(x1, 3);
            double r2 = Math.Round(x2, 3);
            string s1 = r1.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string s2 = r2.ToString(System.Globalization.CultureInfo.InvariantCulture);
            return $"{s1} {s2}";
        }
        catch { return "no roots"; }
    }
}
