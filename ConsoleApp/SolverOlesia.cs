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
            // Убираем всё после = (включая "= ?", "= ?")
            string expr = Regex.Replace(input, @"=.*$", "").Trim();
            var tokens = Tokenize(expr);
            int pos = 0;
            long result = ParseAddSub(tokens, ref pos);
            return result.ToString();
        }
        catch { return "0"; }
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
        if (pos < tokens.Count && tokens[pos] == "+") pos++;
        return ParsePrimary(tokens, ref pos);
    }

    private static long ParsePrimary(List<string> tokens, ref int pos)
    {
        if (pos < tokens.Count && tokens[pos] == "(")
        {
            pos++;
            long val = ParseAddSub(tokens, ref pos);
            if (pos < tokens.Count && tokens[pos] == ")") pos++;
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
            if (c == '(' || c == ')' || c == '*' || c == '/' || c == '%')
            {
                tokens.Add(c.ToString());
                i++;
            }
            else if (c == '+' || c == '-')
            {
                // Унарный если первый токен или после оператора/открывающей скобки
                bool isUnary = tokens.Count == 0 ||
                    tokens[tokens.Count - 1] == "+" ||
                    tokens[tokens.Count - 1] == "-" ||
                    tokens[tokens.Count - 1] == "*" ||
                    tokens[tokens.Count - 1] == "/" ||
                    tokens[tokens.Count - 1] == "%" ||
                    tokens[tokens.Count - 1] == "(";
                if (isUnary && i + 1 < expr.Length && char.IsDigit(expr[i + 1]))
                {
                    // склеиваем знак с числом
                    int start = i;
                    i++;
                    while (i < expr.Length && char.IsDigit(expr[i])) i++;
                    tokens.Add(expr.Substring(start, i - start));
                }
                else
                {
                    tokens.Add(c.ToString());
                    i++;
                }
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

    // polynomial-root: решаем квадратное уравнение
    public static string SolvePolynomial(string input)
    {
        try
        {
            // Нормализуем пробелы
            string s = input.Trim();
            // Убираем правую часть = 0 (или = что угодно)
            s = Regex.Replace(s, @"=.*$", "").Trim();
            // Убираем все пробелы для упрощения разбора
            s = s.Replace(" ", "");
            // Добавляем явный + перед x если нет знака в начале
            if (s.Length > 0 && s[0] != '+' && s[0] != '-') s = "+" + s;

            double a = 0, b = 0, c = 0;

            // Ищем все термы: знак + коэффициент + переменная (x^2, x, или константа)
            var termRegex = new Regex(@"([+-]\d*\.?\d*)(\*?)([xX]\^2|[xX]|)");
            var matches = termRegex.Matches(s);

            foreach (Match m in matches)
            {
                string coefStr = m.Groups[1].Value;
                string varPart = m.Groups[3].Value.ToLower();

                if (string.IsNullOrEmpty(coefStr) && string.IsNullOrEmpty(varPart)) continue;

                // Парсим коэффициент
                double coef;
                if (coefStr == "+" || coefStr == "") coef = 1;
                else if (coefStr == "-") coef = -1;
                else coef = double.Parse(coefStr, System.Globalization.CultureInfo.InvariantCulture);

                if (varPart == "x^2") a += coef;
                else if (varPart == "x") b += coef;
                else if (!string.IsNullOrEmpty(m.Groups[1].Value) && string.IsNullOrEmpty(varPart))
                    c += coef;
            }

            // Если a=0 и b=0 и c=0 — попробуем ещё раз с другим подходом
            if (Math.Abs(a) < 1e-12 && Math.Abs(b) < 1e-12 && Math.Abs(c) < 1e-12)
                return ParsePolynomialFallback(s);

            // Линейное уравнение
            if (Math.Abs(a) < 1e-12)
            {
                if (Math.Abs(b) < 1e-12) return "no roots";
                double root = -c / b;
                return FormatRoot(root);
            }

            double disc = b * b - 4 * a * c;
            if (disc < -1e-9) return "no roots";
            if (Math.Abs(disc) <= 1e-9) return FormatRoot(-b / (2 * a));

            double x1 = (-b + Math.Sqrt(disc)) / (2 * a);
            double x2 = (-b - Math.Sqrt(disc)) / (2 * a);
            if (x1 >= x2) return $"{FormatRoot(x1)} {FormatRoot(x2)}";
            else return $"{FormatRoot(x2)} {FormatRoot(x1)}";
        }
        catch { return "no roots"; }
    }

    private static string ParsePolynomialFallback(string s)
    {
        // Запасной метод — явный поиск каждого терма по regex
        double a = 0, b = 0, c = 0;

        var aMatch = Regex.Match(s, @"([+-]?\d+\.?\d*)[*]?[xX]\^2");
        if (aMatch.Success)
        {
            string v = aMatch.Groups[1].Value;
            a = (v == "+" || v == "") ? 1 : v == "-" ? -1 : double.Parse(v, System.Globalization.CultureInfo.InvariantCulture);
        }

        string noX2 = Regex.Replace(s, @"[+-]?\d*\.?\d*[*]?[xX]\^2", "");
        var bMatch = Regex.Match(noX2, @"([+-]?\d*\.?\d*)[*]?[xX](?!\^)");
        if (bMatch.Success)
        {
            string v = bMatch.Groups[1].Value;
            b = (v == "+" || v == "") ? 1 : v == "-" ? -1 : double.Parse(v, System.Globalization.CultureInfo.InvariantCulture);
        }

        string noX = Regex.Replace(noX2, @"[+-]?\d*\.?\d*[*]?[xX](?!\^)", "");
        var cMatch = Regex.Match(noX, @"([+-]?\d+\.?\d*)");
        if (cMatch.Success)
            c = double.Parse(cMatch.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);

        if (Math.Abs(a) < 1e-12)
        {
            if (Math.Abs(b) < 1e-12) return "no roots";
            return FormatRoot(-c / b);
        }

        double disc = b * b - 4 * a * c;
        if (disc < -1e-9) return "no roots";
        if (Math.Abs(disc) <= 1e-9) return FormatRoot(-b / (2 * a));

        double x1 = (-b + Math.Sqrt(disc)) / (2 * a);
        double x2 = (-b - Math.Sqrt(disc)) / (2 * a);
        if (x1 >= x2) return $"{FormatRoot(x1)} {FormatRoot(x2)}";
        return $"{FormatRoot(x2)} {FormatRoot(x1)}";
    }

    private static string FormatRoot(double x)
    {
        double rounded = Math.Round(x, 2);
        // Если целое — без десятичной точки
        if (Math.Abs(rounded - Math.Round(rounded)) < 1e-9)
            return ((long)Math.Round(rounded)).ToString();
        return rounded.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
    }
}
