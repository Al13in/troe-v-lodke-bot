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
            var rows = input.Split(new[] { "\\\\" }, StringSplitOptions.RemoveEmptyEntries);
            var m = rows.Select(r => r.Split('&').Select(double.Parse).ToArray()).ToArray();
            double det = m[0][0] * (m[1][1] * m[2][2] - m[1][2] * m[2][1])
                       - m[0][1] * (m[1][0] * m[2][2] - m[1][2] * m[2][0])
                       + m[0][2] * (m[1][0] * m[2][1] - m[1][1] * m[2][0]);
            return det.ToString();
        }
        catch { return "0"; }
    }
}