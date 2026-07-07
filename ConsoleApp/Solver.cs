using System;
using System.Text.RegularExpressions;
using Challenge.DataContracts;

namespace ConsoleApp;

public class Solver
{
    public static string Solve(TaskResponse taskResponse)
    {
        string question = taskResponse.Question;

        if (question.Contains("Главный вопрос жизни"))
        {
            return SolverCaptain.SolveStarter(question);
        }

        if (question.Contains("\\\\") || question.Contains("&"))
        {
            return SolverCaptain.SolveMatrix(question);
        }

        if (question.Contains("x^2") || question.Contains("x") || taskResponse.UserHint.Contains("root"))
        {
            return SolverOlesia.SolvePolynomial(question);
        }

        if (Regex.IsMatch(question, @"\d+\s*[\+\-\*\/]\s*\d+"))
        {
            return SolverOlesia.SolveSimpleMath(question);
        }

        if (question.Contains("#reversed#"))
        {
            return SolverJailed.SolveCypher(question);
        }

        if (Regex.IsMatch(question, @"^[IVXLCDM]+\s*\n"))
        {
            return SolverJailed.SolveSteganography(question);
        }

        return "no roots"; 
    }
}