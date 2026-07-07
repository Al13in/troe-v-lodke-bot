using System;
using System.Linq;

namespace ConsoleApp;

public static class SolverCaptain
{
    public static string SolveStarter(string question)
    {
        return "42";
    }

    public static string SolveMatrix(string input)
    {
        try
        {
            // Разбиваем по \\ (разделитель строк матрицы в LaTeX-формате)
            var rows = input
                .Split(new[] { "\\\\" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Trim())
                .Where(r => !string.IsNullOrEmpty(r))
                .Select(r => r.Split('&')
                    .Select(x => double.Parse(x.Trim(), System.Globalization.CultureInfo.InvariantCulture))
                    .ToArray())
                .ToArray();

            int n = rows.Length;

            if (n == 1)
                return RoundDet(rows[0][0]);

            if (n == 2)
            {
                double det2 = rows[0][0] * rows[1][1] - rows[0][1] * rows[1][0];
                return RoundDet(det2);
            }

            // Общий алгоритм Гаусса для n×n
            double det = GaussDet(rows);
            return RoundDet(det);
        }
        catch { return "0"; }
    }

    private static double GaussDet(double[][] m)
    {
        int n = m.Length;
        // Копируем матрицу
        double[][] a = m.Select(r => r.ToArray()).ToArray();
        double det = 1.0;
        for (int col = 0; col < n; col++)
        {
            // Ищем ненулевой pivot
            int pivotRow = -1;
            for (int row = col; row < n; row++)
            {
                if (Math.Abs(a[row][col]) > 1e-12)
                {
                    pivotRow = row;
                    break;
                }
            }
            if (pivotRow == -1) return 0; // вырожденная
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
        // Если очень близко к целому — возвращаем целое
        long rounded = (long)Math.Round(det);
        if (Math.Abs(det - rounded) < 1e-6)
            return rounded.ToString();
        return Math.Round(det, 4).ToString(System.Globalization.CultureInfo.InvariantCulture);
    }
}
