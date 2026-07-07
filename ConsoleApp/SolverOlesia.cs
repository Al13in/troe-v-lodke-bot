using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace ConsoleApp;

public static class SolverOlesia
{
    // math: вычисляем целочисленное выражение с приоритетами (+,-,*,/,%)
    public static string SolveSimpleMath(string input)
    {
        try
        {
            string expr = input.Replace("=", "").Trim();
            long result = EvalExpr(expr, out _);
            return result.ToString();
        }
        catch { return "0"; }
    }

    // Рекурсивный парсер с поддержкой скобок, унарного минуса, +/-/*/%
    private static long EvalExpr(string expr, out int consumed)
    {
        consumed = 0;
        var tokens = Tokenize(expr);
        int pos = 0;
        long result = ParseAddSub(tokens, ref pos);
        consumed = expr.Length;
        return result;
    }

    private static long ParseAddSub(List<string> tokens, ref int pos)
    {
        long left = ParseMulDiv(tokens, ref pos);
        while (pos < tokens.Count && (tokens[pos] == "+" || tokens[pos] == "-"))
        {
            string op = tokens[pos++];
            long right = ParseMulDiv(tokens, ref pos);
            left = op == "+" ? left + right : left - right;
        }
        return left;
    }

    private static long ParseMulDiv(List<string> tokens, ref int pos)
    {
        long left = ParseUnary(tokens, ref pos);
        while (pos < tokens.Count && (tokens[pos] == "*" || tokens[pos] == "/" || tokens[pos] == "%"))
        {
            string op = tokens[pos++];
            long right = ParseUnary(tokens, ref pos);
            if (op == "*") left = left * right;
            else if (op == "/") left = left / right;
            else left = left % right;
        }
        return left;
    }

    private static long ParseUnary(List<string> tokens, ref int pos)
    {
        if (pos < tokens.Count && tokens[pos] == "-")
        {
            pos++;
            return -ParsePrimary(tokens, ref pos);
        }
        if (pos < tokens.Count && tokens[pos] == "+")
        {
            pos++;
        }
        return ParsePrimary(tokens, ref pos);
    }

    private static long ParsePrimary(List<string> tokens, ref int pos)
    {
        if (pos < tokens.Count && tokens[pos] == "(")
        {
            pos++; // skip '('
            long val = ParseAddSub(tokens, ref pos);
            if (pos < tokens.Count && tokens[pos] == ")")
                pos++; // skip ')'
            return val;
        }
        return long.Parse(tokens[pos++]);
    }

    private static List<string> Tokenize(string expr)
    {
        var tokens = new List<string>();
        int i = 0;
        expr = expr.Replace(" ", "");
        while (i < expr.Length)
        {
            char c = expr[i];
            if (c == '(' || c == ')' || c == '+' || c == '-' || c == '*' || c == '/' || c == '%')
            {
                tokens.Add(c.ToString());
                i++;
            }
            else if (char.IsDigit(c))
            {
                int start = i;
                while (i < expr.Length && char.IsDigit(expr[i])) i++;
                tokens.Add(expr.Substring(start, i - start));
            }
            else i++;
        }
        return tokens;
    }

    // polynomial-root: решаем квадратное уравнение, возвращаем оба корня
    public static string SolvePolynomial(string input)
    {
        try
        {
            // Нормализуем: убираем пробелы, знак = и правую часть
            string s = Regex.Replace(input, @"\s+", "");
            s = Regex.Replace(s, @"=.*$", ""); // убрать =0 и всё после

            double a = 0, b = 0, c = 0;

            // Ищем коэффициент a при x^2
            var aMatch = Regex.Match(s, @"([+-]?(?:\d+\.?\d*|\.\d+)?)\*?x\^2");
            if (aMatch.Success)
            {
                string av = aMatch.Groups[1].Value;
                a = av == "" || av == "+" ? 1 : av == "-" ? -1
                    : double.Parse(av, System.Globalization.CultureInfo.InvariantCulture);
            }

            // Ищем коэффициент b при x (не перед x^2)
            // Удаляем x^2 часть перед поиском x
            string noX2 = Regex.Replace(s, @"[+-]?(?:\d+\.?\d*|\.\d+)?\*?x\^2", "");
            var bMatch = Regex.Match(noX2, @"([+-]?(?:\d+\.?\d*|\.\d+)?)\*?x");
            if (bMatch.Success)
            {
                string bv = bMatch.Groups[1].Value;
                b = bv == "" || bv == "+" ? 1 : bv == "-" ? -1
                    : double.Parse(bv, System.Globalization.CultureInfo.InvariantCulture);
            }

            // Ищем свободный член c — последнее число без x
            string noX = Regex.Replace(noX2, @"[+-]?(?:\d+\.?\d*|\.\d+)?\*?x", "");
            var cMatch = Regex.Match(noX, @"([+-]?(?:\d+\.?\d*|\.\d+))");
            if (cMatch.Success)
                c = double.Parse(cMatch.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);

            // Линейное уравнение
            if (Math.Abs(a) < 1e-12)
            {
                if (Math.Abs(b) < 1e-12) return "no roots";
                double root = -c / b;
                return FormatRoot(root);
            }

            double disc = b * b - 4 * a * c;
            if (disc < -1e-9) return "no roots";
            if (Math.Abs(disc) <= 1e-9)
                return FormatRoot(-b / (2 * a));

            double x1 = (-b + Math.Sqrt(disc)) / (2 * a);
            double x2 = (-b - Math.Sqrt(disc)) / (2 * a);
            // Возвращаем в порядке убывания (x1 >= x2)
            if (x1 >= x2)
                return $"{FormatRoot(x1)} {FormatRoot(x2)}";
            else
                return $"{FormatRoot(x2)} {FormatRoot(x1)}";
        }
        catch { return "no roots"; }
    }

    private static string FormatRoot(double x)
    {
        // Округляем до 2 знаков, убираем лишние нули
        double rounded = Math.Round(x, 2);
        return rounded.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
    }
}
