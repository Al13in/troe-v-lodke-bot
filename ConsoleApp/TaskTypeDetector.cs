using System;
using System.Text.RegularExpressions;

namespace ConsoleApp;

/// <summary>
/// Определяет тип задачи по содержимому вопроса
/// </summary>
public static class TaskTypeDetector
{
    public static string DetectTaskType(string question, string typeId)
    {
        // Если сервер уже сказал тип, используем его
        if (!string.IsNullOrEmpty(typeId) && typeId != "unknown")
            return typeId;

        // Иначе определяем по содержимому вопроса
        if (string.IsNullOrEmpty(question))
            return "unknown";

        question = question.ToLower();

        // Детерминант / матрица - содержит \\ и &
        if ((question.Contains("\\\\") && question.Contains("&")) || 
            question.Contains("determinant") || 
            question.Contains("матриц"))
            return "determinant";

        // Обратная матрица
        if (question.Contains("inverse") || question.Contains("обратн"))
            return "inverse-matrix";

        // Многочлен - содержит x^2, x, = 0 или переменные
        if (Regex.IsMatch(question, @"[xх]\^2|[xх]\^|[xх]\s*=|=\s*0") && 
            !question.Contains("||"))
            return "polynomial-root";

        // Шифр - содержит #reversed# или reversed:
        if (Regex.IsMatch(question, @"#[Rr][Ee][Vv][Ee][Rr][Ss][Ee][Dd]#|[Rr]eversed[:=]"))
            return "cypher";

        // Стеганография - первая строка содержит римские цифры, остальное - текст
        var lines = question.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);
        if (lines.Length > 1)
        {
            var firstLine = lines[0].Trim();
            if (Regex.IsMatch(firstLine, @"^[IVXLCDMivxlcdm]+$") && firstLine.Length <= 10)
                return "steganography";
        }

        // Форма - содержит координаты в скобках (x, y)
        if (Regex.IsMatch(question, @"\(\s*[-\d]+\s*,\s*[-\d]+\s*\)") && 
            question.Contains("|"))
            return "shape";

        // Статистика - содержит |, функции типа sum, min, max, firstmostfrequent
        if (Regex.IsMatch(question, @"\|\s*[-\d\s]+$|sum|min|max|firstmostfrequent"))
            return "statistics";

        // JSON - содержит фигурные скобки или много чисел
        if (question.Contains("{") && question.Contains("}"))
            return "json";

        // Простая математика - арифметическое выражение с операторами
        if (Regex.IsMatch(question, @"[-\d\s+\-*/%()]+\s*=\s*\?") || 
            Regex.IsMatch(question, @"^\d+\s*[+\-*/%]\s*\d+"))
            return "math";

        // Строка-число - текстовое представление чисел (one, two, twenty и т.д.)
        if (Regex.IsMatch(question, @"\b(zero|one|two|three|four|five|six|seven|eight|nine|ten|" +
            @"eleven|twelve|thirteen|fourteen|fifteen|sixteen|seventeen|eighteen|nineteen|" +
            @"twenty|thirty|forty|fifty|sixty|seventy|eighty|ninety|hundred|thousand|million|billion)\b"))
            return "string-number";

        // Момент времени - дата и время
        if (Regex.IsMatch(question, @"\d{2}:\d{2}.*\d{2}\.\d{2}"))
            return "moment";

        return "unknown";
    }
}
