using System;
using Challenge;
using Challenge.DataContracts;
using ConsoleApp;
using Task = System.Threading.Tasks.Task;

const string teamSecret = "SDQzuAEb16bA1BTFVuZ/kkC11NUcd"; 
if (string.IsNullOrEmpty(teamSecret)) return;

var challengeClient = new ChallengeClient(teamSecret);
const string challengeId = "git-course";

// Вот тут теперь ВСЕ 6 типов задач, бот будет перебирать их по очереди!
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

Console.WriteLine("Бот запущен в полностью автоматическом режиме.");

foreach (var taskType in taskTypes)
{
    Console.WriteLine($"\n=== НАЧИНАЕМ РЕШАТЬ ТИП: {taskType} ===");
    int errorsInARow = 0;

    while (true)
    {
        try
        {
            var newTask = await challengeClient.AskNewTaskAsync(currentRound, taskType);
            
            if (newTask == null)
            {
                Console.WriteLine($"Задачи типа {taskType} закончились. Переходим к следующему типу...");
                break;
            }

            string answer = Solver.Solve(newTask);

            var updatedTask = await challengeClient.CheckTaskAnswerAsync(newTask.Id, answer);
            Console.WriteLine($"[{taskType}] Задача {newTask.Id.ToString()[..8]}... Статус: {updatedTask.Status}. Ответ: {answer}");
            
            errorsInARow = 0; 
            await Task.Delay(500);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обработке {taskType}: {ex.Message}");
            errorsInARow++;
            
            if (errorsInARow >= 3)
            {
                Console.WriteLine($"Слишком много ошибок для {taskType}. Идем дальше.");
                break;
            }
            await Task.Delay(2000);
        }
    }
}

Console.WriteLine("\nВсе доступные типы задач обработаны!");