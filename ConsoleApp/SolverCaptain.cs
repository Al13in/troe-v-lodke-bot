using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp;

public static class SolverCaptain
{
    public static string SolveStarter(string question)
    {
        return "42";
    }

    // ---- DETERMINANT ----
    public static string SolveMatrix(string input)
    {
        try
        {
            var rows = input
                .Split(new[] { "\\\\" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Trim())
                .Where(r => !string.IsNullOrEmpty(r))
                .Select(r => r.Split('&')
                .Select(x => double.Parse(x.Trim(), System.Globalization.CultureInfo.InvariantCulture))
                .ToArray())
                .ToArray();
            int n = rows.Length;
            if (n == 1) return RoundDet(rows[0][0]);
            if (n == 2)
            {
                double det2 = rows[0][0] * rows[1][1] - rows[0][1] * rows[1][0];
                return RoundDet(det2);
            }
            return RoundDet(GaussDet(rows));
        }
        catch { return "0"; }
    }

    private static double GaussDet(double[][] m)
    {
        int n = m.Length;
        double[][] a = m.Select(r => r.ToArray()).ToArray();
        double det = 1.0;
        for (int col = 0; col < n; col++)
        {
            int pivotRow = -1;
            for (int row = col; row < n; row++)
                if (Math.Abs(a[row][col]) > 1e-12) { pivotRow = row; break; }
            if (pivotRow == -1) return 0;
            if (pivotRow != col)
            {
                var tmp = a[col]; a[col] = a[pivotRow]; a[pivotRow] = tmp;
                det *= -1;
            }
            det *= a[col][col];
            for (int row = col + 1; row < n; row++)
            {
                double factor = a[row][col] / a[col][col];
                for (int k = col; k < n; k++)
                    a[row][k] -= factor * a[col][k];
            }
        }
        return det;
    }

    private static string RoundDet(double det)
    {
        long rounded = (long)Math.Round(det);
        if (Math.Abs(det - rounded) < 1e-6) return rounded.ToString();
        return Math.Round(det, 4).ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    // ---- INVERSE MATRIX ----
    public static string SolveInverseMatrix(string input)
    {
        try
        {
            var list = input.Split(" \\\\ ");
            var matrix = new List<List<BigInteger>>();
            foreach (var row in list)
            {
                var r = new List<BigInteger>();
                foreach (var cell in row.Split(" & "))
                    r.Add(BigInteger.Parse(cell.Trim()));
                matrix.Add(r);
            }
            var det = Opredelitel(matrix);
            if (det == 0) return "unsolvable";
            var adj = MatrixAlgebrDop(matrix);
            var trans = Transpose(adj);
            double invDet = (double)1 / (double)det;
            var result = MultiplexorDouble(trans, invDet);
            var sb = new StringBuilder();
            for (int i = 0; i < result.Count; i++)
            {
                for (int j = 0; j < result[i].Count; j++)
                {
                    if (j > 0) sb.Append(" & ");
                    sb.Append(FormatDouble(result[i][j]));
                }
                if (i < result.Count - 1) sb.Append(" \\\\ ");
            }
            return sb.ToString();
        }
        catch { return "unsolvable"; }
    }

    private static BigInteger Opredelitel(List<List<BigInteger>> matrix)
    {
        if (matrix.Count == 2)
            return matrix[0][0] * matrix[1][1] - matrix[0][1] * matrix[1][0];
        BigInteger ans = 0;
        BigInteger flag = 1;
        for (int z = 0; z < matrix.Count; z++)
        {
            var matrixLess = new List<List<BigInteger>>();
            for (int i = 0; i < matrix.Count; i++)
            {
                if (i == z) continue;
                var row = new List<BigInteger>();
                for (int j = 1; j < matrix[i].Count; j++)
                    row.Add(matrix[i][j]);
                matrixLess.Add(row);
            }
            ans += Opredelitel(matrixLess) * matrix[z][0] * flag;
            flag = -flag;
        }
        return ans;
    }

    private static List<List<BigInteger>> MatrixAlgebrDop(List<List<BigInteger>> matrix)
    {
        int n = matrix.Count;
        var result = new List<List<BigInteger>>();
        for (int i = 0; i < n; i++)
        {
            var row = new List<BigInteger>();
            for (int j = 0; j < n; j++)
            {
                var minor = new List<List<BigInteger>>();
                for (int r = 0; r < n; r++)
                {
                    if (r == i) continue;
                    var mr = new List<BigInteger>();
                    for (int c = 0; c < n; c++)
                    {
                        if (c == j) continue;
                        mr.Add(matrix[r][c]);
                    }
                    minor.Add(mr);
                }
                BigInteger sign = ((i + j) % 2 == 0) ? 1 : -1;
                row.Add(sign * Opredelitel(minor));
            }
            result.Add(row);
        }
        return result;
    }

    private static List<List<BigInteger>> Transpose(List<List<BigInteger>> matrix)
    {
        int n = matrix.Count;
        var result = new List<List<BigInteger>>();
        for (int i = 0; i < n; i++)
        {
            var row = new List<BigInteger>();
            for (int j = 0; j < n; j++)
                row.Add(matrix[j][i]);
            result.Add(row);
        }
        return result;
    }

    private static List<List<double>> MultiplexorDouble(List<List<BigInteger>> matrix, double factor)
    {
        var result = new List<List<double>>();
        foreach (var row in matrix)
        {
            var r = new List<double>();
            foreach (var val in row)
                r.Add((double)val * factor);
            result.Add(r);
        }
        return result;
    }

    private static string FormatDouble(double v)
    {
        long rounded = (long)Math.Round(v);
        if (Math.Abs(v - rounded) < 1e-6) return rounded.ToString();
        return Math.Round(v, 4).ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    // ---- STATISTICS ----
    public static string SolveStatistics(string question)
    {
        try
        {
            int sep = question.IndexOf('|');
            var funcType = question.Substring(0, sep);
            var inputNumbers = question.Substring(sep + 1).Trim().Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
            var numbers = inputNumbers.Select(int.Parse).ToList();
            if (funcType == "sum") return numbers.Sum().ToString();
            if (funcType == "min") return numbers.Min().ToString();
            if (funcType == "max") return numbers.Max().ToString();
            if (funcType == "firstmostfrequent")
                return numbers.GroupBy(x => x).OrderByDescending(x => x.Count()).ThenBy(x => numbers.IndexOf(x.Key)).First().Key.ToString();
            return "";
        }
        catch { return ""; }
    }

    // ---- JSON (sum all numbers) ----
    public static string SolveJson(string question)
    {
        try
        {
            var jsonValues = new List<long>();
            var curNum = "";
            foreach (char c in question)
            {
                if (char.IsDigit(c) || c == '-')
                    curNum += c;
                else
                {
                    if (curNum != "") { jsonValues.Add(long.Parse(curNum)); curNum = ""; }
                }
            }
            if (curNum != "") jsonValues.Add(long.Parse(curNum));
            return jsonValues.Sum().ToString();
        }
        catch { return "0"; }
    }

    // ---- STRING-NUMBER ----
    private static readonly Dictionary<string, long> NumberTable = new()
    {
        {"zero",0},{"one",1},{"two",2},{"three",3},{"four",4},{"five",5},{"six",6},
        {"seven",7},{"eight",8},{"nine",9},{"ten",10},{"eleven",11},{"twelve",12},
        {"thirteen",13},{"fourteen",14},{"fifteen",15},{"sixteen",16},{"seventeen",17},
        {"eighteen",18},{"nineteen",19},{"twenty",20},{"thirty",30},{"forty",40},
        {"fifty",50},{"sixty",60},{"seventy",70},{"eighty",80},{"ninety",90},
        {"hundred",100},{"thousand",1000},{"million",1000000},
        {"billion",1000000000},{"trillion",1000000000000L},{"quadrillion",1000000000000000L}
    };

    public static string SolveStringNumber(string question)
    {
        try
        {
            var words = Regex.Matches(question, @"\w+")
                .Cast<Match>()
                .Select(m => m.Value.ToLowerInvariant())
                .Where(v => NumberTable.ContainsKey(v))
                .Select(v => NumberTable[v]);
            long result = 0, current = 0;
            foreach (var n in words)
            {
                if (n == 100) current *= 100;
                else if (n >= 1000) { result += (current == 0 ? 1 : current) * n; current = 0; }
                else current += n;
            }
            return (result + current).ToString();
        }
        catch { return "0"; }
    }

    // ---- SHAPE ----
    public static string SolveShape(string question)
    {
        try
        {
            int idx = question.IndexOf('|');
            var coordPart = question.Substring(idx + 1).Trim();
            var pointStrings = coordPart.Split(' ');
            var points = new List<(int x, int y)>();
            foreach (var ps in pointStrings)
            {
                var clean = ps.Trim('(', ')');
                var parts = clean.Split(',');
                points.Add((int.Parse(parts[0].Trim()), int.Parse(parts[1].Trim())));
            }
            int minY = points.Min(p => p.y);
            var bottomPoints = points.Where(p => p.y == minY).ToList();
            if (bottomPoints.Count == 2) return "equilateraltriangle";
            int maxX = points.Max(p => p.x);
            var rightMost = points.Where(p => p.x == maxX).OrderBy(p => p.y).First();
            var bottomRight = bottomPoints.OrderByDescending(p => p.x).First();
            if (rightMost == bottomRight) return "square";
            return "circle";
        }
        catch { return ""; }
    }

    // ---- MOMENT ----
    public static string SolveMoment(string question)
    {
        try
        {
            string[] months = { "января","февраля","марта","апреля","мая",
                "июня","июля","августа","сентября","октября","ноября","декабря" };
            var month = months[int.Parse(question.Substring(12, 2)) - 1];
            var hour = question.Substring(0, 2);
            var minute = question.Substring(3, 2);
            var day = int.Parse(question.Substring(9, 2)).ToString();
            return day + " " + month + " " + hour + ":" + minute;
        }
        catch { return ""; }
    }
}
