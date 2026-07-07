using System;
using Challenge;
using Challenge.DataContracts;
using ConsoleApp;
using Task = System.Threading.Tasks.Task;

const string teamSecret = "SDQzuAEb16bA1BTFVuZ/kkC11NUcd"; 
if (string.IsNullOrEmpty(teamSecret)) return;

var challengeClient = new ChallengeClient(teamSecret);
const string challengeId = "git-course";
const string taskType = "determinant"; 

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
    Console.WriteLine("Текущий раунд не найден! Возможно, этап еще не начался.");
    return;
}

Console.WriteLine($"Бот запущен. Автоматически решаем задачи типа: {taskType}...");

while (true)
{
    try
    {
        var newTask = await challengeClient.AskNewTaskAsync(currentRound, taskType);
        
        if (newTask == null)
        {
            Console.WriteLine("Задачи закончились или сервер не выдал новую. Ждем 2 секунды...");
            await Task.Delay(2000);
            continue;
        }

        string answer = Solver.Solve(newTask);
        var updatedTask = await challengeClient.CheckTaskAnswerAsync(newTask.Id, answer);

        Console.WriteLine($"Задача {newTask.Id.ToString()[..8]}... Статус: {updatedTask.Status}. Ответ: {answer}");
        
        await Task.Delay(500);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}. Перезапуск через 3 секунды...");
        await Task.Delay(3000);
    }
}