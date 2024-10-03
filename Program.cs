using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Domain;
using TaskTracker.Enums;
using TaskTracker.Interfaces;
using TaskTracker.Services;
using TaskTracker.Utils;

var serviceCollection = new ServiceCollection();
ConfigureServices(serviceCollection);
var serviceProvider = serviceCollection.BuildServiceProvider();
var _taskService = serviceProvider.GetService<ITaskService>();

DisplayWelcomeMessage();
List<string> commands = [];

while (true)
  {
  ConsolePrintConfig.PrintCommandMsg("Enter Command: ");
  string input = Console.ReadLine();

  if (string.IsNullOrEmpty(input))
    {
    ConsolePrintConfig.PrintErrorMsg("\n Not a known command, try again");
    ConsolePrintConfig.PrintInfoMsg("Type \"help\" to show commands.");
    continue;
    }

  commands = ConsolePrintConfig.ParseInput(input);

  string command = commands[0].ToLower();

  bool exit = false;

  switch (command)
    {
    case "help":
      PrintHelpCommands();
      break;

    case "add":
      AddNewTask();
      break;

    case "delete":
      DeleteTask();
      break;

    case "update":
      UpdateTask();
      break;

    case "list":
      DisplayAllTasks();
      break;

    case "clear":
      ConsolePrintConfig.ClearConsole();
      DisplayWelcomeMessage();
      continue;

    case "mark-in-progress":
      SetStatusOfTask();
      break;

    case "mark-todo":
      SetStatusOfTask();
      break;

    case "mark-done":
      SetStatusOfTask();
      break;

    case "exit":
      exit = true;
      break;

    default:
      break;
    }

  if (exit)
    {
    break;
    }

  }

void PrintHelpCommands()
  {
  var helpCommands = _taskService?.GetHelpCommands();
  int count = 1;

  if (helpCommands != null)
    {
    foreach (var cmd in helpCommands)
      {
      ConsolePrintConfig.PrintHelpMsg(count + " - " + cmd);
      count++;
      }
    }
  }

void AddNewTask()
  {
  if (!IsUserInputValid(commands, 2))
    {
    return;
    }

  var taskAdded = _taskService?.AddNewTask(commands[1]);

  if (taskAdded != null && taskAdded.Result != 0)
    {
    ConsolePrintConfig.PrintInfoMsg($"Task added successfully with Id: {taskAdded.Result}.");
    }
  else
    ConsolePrintConfig.PrintInfoMsg("Task not saved1!");
  }

void DeleteTask()
  {
  if (!IsUserInputValid(commands, 2))
    {
    return;
    }

  int id = IsValidIdProvided(commands, 0).Item2;

  if (id == 0)
    {
    return;
    }

  var result = _taskService?.DeleteTask(id).Result;

  if (result != null && result.Value)
    ConsolePrintConfig.PrintInfoMsg($"Task deleted successfully with Id : {id}");
  else
    ConsolePrintConfig.PrintInfoMsg($"Task with Id : {id}, does not exist!");

  }

void UpdateTask()
  {
  if (!IsUserInputValid(commands, 3))
    {
    return;
    }

  int id = IsValidIdProvided(commands, 0).Item2;

  if (id == 0)
    {
    return;
    }

  var result = _taskService?.UpdateTask(id, commands[2]).Result;

  if (result != null && result.Value)
    {
    ConsolePrintConfig.PrintInfoMsg($"Task updated successfully with Id : {id}");
    }
  else
    {
    ConsolePrintConfig.PrintInfoMsg($"Task with Id : {id}, does not exist!");
    }

  }

void DisplayAllTasks()
  {
  if (commands.Count > 2)
    {
    ConsolePrintConfig.PrintErrorMsg("Wrong command! Try again.");
    ConsolePrintConfig.PrintInfoMsg("Type \"help\" to know the set of commands");
    }

  List<ToDoTask> tasks = new List<ToDoTask>();

  if (commands.Count == 1)
    {
    tasks = _taskService?.GetAllTasks().Result.OrderBy(task => task.Id).ToList() ?? tasks;
    }
  else
    {
    if (!commands[1].ToLower().Equals("in-progress") && !commands[1].ToLower().Equals("done") && !commands[1].ToLower().Equals("todo"))
      {
      ConsolePrintConfig.PrintErrorMsg("Wrong command! Try again.");
      ConsolePrintConfig.PrintInfoMsg("Type \"help\" to know the set of commands");
      }
    tasks = _taskService.GetTaskByStatus(commands[1]).Result.OrderBy(task => task.Id).ToList() ?? tasks;
    }
  CreateTaskTable(tasks);
  }

void CreateTaskTable(List<ToDoTask> tasks)
  {
  int colWidth = 15, colWidth2 = 35, colWidth3 = 15, colWidth4 = 15;

  if (tasks != null && tasks.Count > 0)
    {
    Console.WriteLine("\n{0,-" + colWidth + "} {1,-" + colWidth2 + "} {2,-" + colWidth3 + "} {3,-" + colWidth4 + "}",
        "Task Id", "Description", "Status", "Created Date" + "\n");

    foreach (ToDoTask task in tasks)
      {
      SetConsoleTextColor(task);
      Console.WriteLine("{0,-" + colWidth + "} {1,-" + colWidth2 + "} {2,-" + colWidth3 + "} {3,-" + colWidth4 + "}"
          , task.Id, task.Description, task.Status, task.CreatedAt.Date.ToString("dd-MM-yyyy"));
      Console.ResetColor();
      }
    }
  else
    {
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("\n No Tasks registered!\n");
    Console.ResetColor();
    Console.WriteLine("{0,-\" + colWidth1 + \"} {1,-\" + colWidth2 + \"} {2,-\" + colWidth3 + \"} {3,-\" + colWidth4 + \"}\",\r\n           \"Task Id\", \"Description\", \"Status\", \"CreatedDate");
    }
  }

void SetStatusOfTask()
  {
  if (!IsUserInputValid(commands, 2))
    {
    return;
    }
  int id = IsValidIdProvided(commands, 0).Item2;

  if (id == 0)
    {
    return;
    }

  var result = _taskService?.SetStatus(id, commands[0]).Result;

  if (result != null && result.Value)
    {
    ConsolePrintConfig.PrintInfoMsg($"Task status set successfully with Id : {id}");
    }
  else
    {
    ConsolePrintConfig.PrintInfoMsg($"Task with Id : {id}, does not exist!");
    }
  }

static Tuple<bool, int> IsValidIdProvided(List<string> commands, int id)
  {
  Int32.TryParse(commands[1], out id);

  if (id == 0)
    {
    ConsolePrintConfig.PrintErrorMsg("Wrong command! Try again.");
    ConsolePrintConfig.PrintInfoMsg("Type \"help\" to know the set of commands");
    return new Tuple<bool, int>(false, id);
    }

  return new Tuple<bool, int>(true, id);
  }

static bool IsUserInputValid(List<string> commands, int parameter)
  {
  bool validInput = true;

  if (parameter == 1)
    {
    if (commands.Count != parameter)
      {
      validInput = false;
      }
    }

  if (parameter == 2)
    {
    if (commands.Count != parameter || string.IsNullOrEmpty(commands[1]))
      {
      validInput = false;
      }
    }

  if (parameter == 3)
    {
    if (commands.Count != parameter || string.IsNullOrEmpty(commands[1]) || string.IsNullOrEmpty(commands[2]))
      {
      validInput = false;
      }
    }

  if (!validInput)
    {

    ConsolePrintConfig.PrintErrorMsg("Wrong command! Try again.");
    ConsolePrintConfig.PrintInfoMsg("Type \"help\" to know the set of commands");
    return false;
    }

  return true;
  }

void DisplayWelcomeMessage()
  {
  ConsolePrintConfig.PrintInfoMsg("Hi, Welcome to your Tasks App!");
  ConsolePrintConfig.PrintInfoMsg("Type \"help\" to show commands.");
  }

static void SetConsoleTextColor(ToDoTask task)
  {
  if (task.Status == StatusEnum.toDo)
    {
    Console.ForegroundColor = ConsoleColor.Magenta;
    }
  else if (task.Status == StatusEnum.done)
    {
    Console.ForegroundColor = ConsoleColor.Green;
    }
  else
    {
    Console.ForegroundColor = ConsoleColor.Yellow;
    }
  }

static void ConfigureServices(ServiceCollection serviceCollection)
  {
  serviceCollection.AddSingleton<ITaskService, TaskService>();
  }

