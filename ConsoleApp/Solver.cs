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
            "starter"                  => SolverCaptain.SolveStarter(question),
            "determinant"             => SolverCaptain.SolveMatrix(question),
            "inverse-matrix"          => SolverCaptain.SolveInverseMatrix(question),
            "statistics"              => SolverCaptain.SolveStatistics(question),
            "statistics-composition"  => SolverCaptain.SolveStatistics(question),
            "json"                    => SolverCaptain.SolveJson(question),
            "newton-json"             => SolverCaptain.SolveJson(question),
            "string-number"           => SolverCaptain.SolveStringNumber(question),
            "shape"                   => SolverCaptain.SolveShape(question),
            "moment"                  => SolverCaptain.SolveMoment(question),
            "math"                    => SolverOlesia.SolveSimpleMath(question),
            "random-math"             => SolverOlesia.SolveSimpleMath(question),
            "polynomial-root"         => SolverOlesia.SolvePolynomial(question),
            "cypher"                  => SolverJailed.SolveCypher(question),
            "taare-zameen-par-cypher" => SolverJailed.SolveCypher(question),
            "steganography"           => SolverJailed.SolveSteganography(question),
            "japanese-steganography"  => SolverJailed.SolveSteganography(question),
            "japanese"                => SolverJailed.SolveSteganography(question),
            _                         => ""
        };
    }
}
