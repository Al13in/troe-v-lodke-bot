using System;
using System.Text.RegularExpressions;
using Challenge.DataContracts;

namespace ConsoleApp;

public class Solver
{
    public static string Solve(TaskResponse taskResponse)
    {
        string question = taskResponse.Question;
        string typeId = taskResponse.TypeId ?? "";
        string hint = taskResponse.UserHint ?? "";

        return typeId switch
        {
            "starter"        => SolverCaptain.SolveStarter(question),
            "determinant"    => SolverCaptain.SolveMatrix(question),
            "math"           => SolverOlesia.SolveSimpleMath(question),
            "polynomial-root"=> SolverOlesia.SolvePolynomial(question),
            "cypher"         => SolverJailed.SolveCypher(question),
            "steganography"  => SolverJailed.SolveSteganography(question),
            _                => "no roots"
        };
    }
}
