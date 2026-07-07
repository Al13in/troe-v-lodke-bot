using System;
using Challenge;
using Challenge.DataContracts;
using ConsoleApp;
using Task = System.Threading.Tasks.Task;

const string teamSecret = "SDQzuAEb16bA1BTFVuZ/kkC11NUcd";
if (string.IsNullOrEmpty(teamSecret)) return;

var challengeClient = new ChallengeClient(teamSecret);
const string challengeId = "git-course";

string[] taskTypes = { "starter", "math", "determinant", "polynomial-root", "cypher", "steganography" };

var challenge = await challengeClient.GetChallengeAsync(challengeId);
var utcNow = DateTime.UtcNow;
string currentRound = null;

foreach (var round in challenge.Rounds)
{
    if (round.StartTimestamp < utcNow && utcNow < round.EndTimestamp)
        currentRound = round.Id;
}

if (string.IsNullOrEmpty(currentRound))
{
    Console.WriteLine("Текущий раунд не найден!");
    return;
}

Console.WriteLine($"Бот запущен. Round={currentRound}");

foreach (var taskType in taskTypes)
{
    Console.WriteLine($"\n=== НАЧИНАЕМ РЕШАТЬ ТИП: {taskType} ===");
    int errorsInARow = 0;
    int successCount = 0;
    int failCount = 0;

    while (true)
    {
        try
        {
            var newTask = await challengeClient.AskNewTaskAsync(currentRound, taskType);

            if (newTask == null)
            {
                Console.WriteLine($"Задачи типа {taskType} закончились.");
                break;
            }

            string answer = Solver.Solve(newTask);
            var updatedTask = await challengeClient.CheckTaskAnswerAsync(newTask.Id, answer);

            bool ok = (int)updatedTask.Status == 1;
            if (ok) successCount++; else failCount++;

            string icon = ok ? "[OK]" : "[FAIL]";
            string q = newTask.Question ?? "";
            string qShort = q.Replace("\n", " ").Replace("\r", "");
            if (qShort.Length > 60) qShort = qShort.Substring(0, 60);
            Console.WriteLine($"{icon} [{taskType}] Q: {qShort} | A: {answer} | Status: {(int)updatedTask.Status}");

            errorsInARow = 0;
            await Task.Delay(300);
        }
        catch (Exception ex)
        {
            if (ex is ErrorResponseException apiEx && (int)apiEx.StatusCode == 400)
            {
                Console.WriteLine($"Задачи {taskType} исчерпаны. OK={successCount} FAIL={failCount}");
                break;
            }

            Console.WriteLine($"Ошибка [{taskType}]: {ex.GetType().Name}: {ex.Message}");
            errorsInARow++;
            if (errorsInARow >= 5)
            {
                Console.WriteLine($"Слишком много ошибок для {taskType}. Идем дальше.");
                break;
            }
            await Task.Delay(2000);
        }
    }
}

Console.WriteLine("\nВсе доступные типы задач обработаны!");
