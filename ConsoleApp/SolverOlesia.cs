using System;
using System.Text.RegularExpressions;
using System.Data;

namespace ConsoleApp;

public static class SolverOlesia
{
    // math: вычисляем целочисленное выражение с приоритетами (+,-,*,/,%)
    public static string SolveSimpleMath(string input)
    {
        try
        {
            string expr = input.Replace("=", "").Trim();
            long result = EvalExpr(expr);
            return result.ToString();
        }
        catch { return "0"; }
    }

    private static long EvalExpr(string expr)
    {
        // Рекурсивный вычислитель: сначала */%  потом +/-
        expr = expr.Trim();
        // Ищем последний + или - вне скобок
        for (int i = expr.Length - 1; i >= 0; i--)
        {
            char c = expr[i];
            if ((c == '+' || c == '-') && i > 0)
            {
                long left = EvalExpr(expr.Substring(0, i));
                long right = EvalTerm(expr.Substring(i + 1));
                return c == '+' ? left + right : left - right;
            }
        }
        return EvalTerm(expr);
    }

    private static long EvalTerm(string expr)
    {
        expr = expr.Trim();
        // Ищем последние * / % 
        for (int i = expr.Length - 1; i >= 0; i--)
        {
            char c = expr[i];
            if (c == '*' || c == '/' || c == '%')
            {
                long left = EvalTerm(expr.Substring(0, i));
                long right = long.Parse(expr.Substring(i + 1).Trim());
                if (c == '*') return left * right;
                if (c == '/') return left / right;
                if (c == '%') return left % right;
            }
        }
        return long.Parse(expr.Trim());
    }

    // polynomial-root: решаем квадратное уравнение, возвращаем оба корня
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
            if (a == 0 && b != 0)
                return Math.Round(-c / b, 3).ToString(System.Globalization.CultureInfo.InvariantCulture);
            double d = b * b - 4 * a * c;
            if (d < 0) return "no roots";
            double x1 = (-b + Math.Sqrt(d)) / (2 * a);
            double x2 = (-b - Math.Sqrt(d)) / (2 * a);
            if (Math.Abs(d) < 1e-9)
                return Math.Round(x1, 3).ToString(System.Globalization.CultureInfo.InvariantCulture);
            string s1 = Math.Round(x1, 3).ToString(System.Globalization.CultureInfo.InvariantCulture);
            string s2 = Math.Round(x2, 3).ToString(System.Globalization.CultureInfo.InvariantCulture);
            return $"{s1} {s2}";
        }
        catch { return "no roots"; }
    }
}
