using System.Text.Json;
using TaskTracker.Domain;
using TaskTracker.Enums;
using TaskTracker.Interfaces;

namespace TaskTracker.Services
  {
  public class TaskService : ITaskService
    {

    private static string FileName = "task_data.json";

    private static string FilePath = Path.Combine(Directory.GetCurrentDirectory(), FileName);

    public Task<int> AddNewTask(string description)
      {
      try
        {
        List<ToDoTask> toDoTasks = new List<ToDoTask>();
        ToDoTask task = new ToDoTask
          {
          Id = GetTaskId(),
          Description = description,
          CreatedAt = DateTime.Now,
          UpdatedAt = DateTime.Now,
          Status = StatusEnum.toDo
          };

        var fileCreatedSuccessfully = CreateFileIfNotExists();

        if (fileCreatedSuccessfully)
          {
          string tasksFromJson = File.ReadAllText(FilePath);
          if (!string.IsNullOrEmpty(tasksFromJson))
            {
            toDoTasks = JsonSerializer.Deserialize<List<ToDoTask>>(tasksFromJson);
            }

          toDoTasks?.Add(task);
          string updatedToDoTasks = JsonSerializer.Serialize<List<ToDoTask>>(toDoTasks ?? new List<ToDoTask>());
          File.WriteAllText(FilePath, updatedToDoTasks);
          return Task.FromResult(task.Id);
          }

        return Task.FromResult(0);
        }
      catch (Exception ex)
        {
        Console.WriteLine($"Task addition failed. Error - " + ex.Message);
        return Task.FromResult(0);
        }
      }

    private int GetTaskId()
      {
      if (!File.Exists(FilePath))
        {
        return 1;
        }
      else
        {
        string taskFromJson = File.ReadAllText(FilePath);
        if (!string.IsNullOrEmpty(taskFromJson))
          {
          var toDoTasks = JsonSerializer.Deserialize<List<ToDoTask>>(taskFromJson);
          if (toDoTasks != null && toDoTasks.Count > 0)
            {
            return toDoTasks.OrderBy(task => task.Id).Last().Id + 1;
            }
          }
        }
      return 1;
      }

    public Task<bool> DeleteTask(int id)
      {
      if (!File.Exists(FilePath))
        {
        return Task.FromResult(false);
        }

      var taskFromJson = GetTaskFromJson();

      if (taskFromJson.Result.Count > 0)
        {
        var taskToBeDeleted = taskFromJson.Result
          .Where(task => task.Id == id)
          .SingleOrDefault();

        if (taskToBeDeleted != null)
          {
          taskFromJson.Result.Remove(taskToBeDeleted);
          UpdateJsonFile(taskFromJson);
          return Task.FromResult(true);
          }
        }
      return Task.FromResult(false);
      }

    private void UpdateJsonFile(Task<List<ToDoTask>> taskFromJson)
      {
      string updatedAppTasks = JsonSerializer.Serialize<List<ToDoTask>>(taskFromJson.Result);
      }

    private static Task<List<ToDoTask>> GetTaskFromJson()
      {
      string tasksFromJsonFile = File.ReadAllText(FilePath);

      if (!string.IsNullOrEmpty(tasksFromJsonFile))
        {
        return Task.FromResult(JsonSerializer.Deserialize<List<ToDoTask>>(tasksFromJsonFile) ?? new List<ToDoTask>());
        }
      return Task.FromResult(new List<ToDoTask>());
      }

    public Task<List<ToDoTask>> GetAllTasks()
      {
      try
        {
        if (!File.Exists(FilePath))
          {
          return Task.FromResult(new List<ToDoTask>());
          }

        string jsonString = File.ReadAllText(FilePath);

        if (!string.IsNullOrEmpty(jsonString))
          {
          List<ToDoTask> tasks = JsonSerializer.Deserialize<List<ToDoTask>>(jsonString);
          return Task.FromResult<List<ToDoTask>>(tasks ?? new List<ToDoTask>());
          }
        else
          {
          return Task.FromResult(new List<ToDoTask>());
          }
        }
      catch (Exception)
        {

        throw;
        }
      }

    public List<string> GetHelpCommands()
      {
      return new List<string>
            {
                "add \"Task Description\" - To add a new task, type add with task description",
                "update \"Task Id\" \"Task Description\" - To update a task, type update with task id and task description",
                "delete \"Task Id\" - To delete a task, type delete with task id",
                "mark-in-progress \"Task Id\" - To mark a task to in progress, type mark-in-progress with task id",
                "mark-done \"Task Id\" - To mark a task to done, type mark-done with task id",
                "list - To list all task with its current status",
                "list done - To list all task with done status",
                "list todo  - To list all task with todo status",
                "list in-progress  - To list all task with in-progress status",
                "exit - To exit from app",
                "clear - To clear console window"
            };
      }

    public Task<List<ToDoTask>> GetTaskByStatus(string status)
      {
      try
        {
        if (!File.Exists(FilePath))
          {
          return Task.FromResult(new List<ToDoTask>());
          }

        string jsonString = File.ReadAllText(FilePath);

        if (!string.IsNullOrEmpty(jsonString))
          {
          List<ToDoTask> tasks = JsonSerializer.Deserialize<List<ToDoTask>>(jsonString);
          var statusToCheck = GetStatusToDisplay(status);
          return Task.FromResult(tasks?.Where(task => task.Status == statusToCheck).ToList() ?? new List<ToDoTask>());
          }
        else
          {
          return Task.FromResult(new List<ToDoTask>());
          }
        }
      catch (Exception)
        {

        throw;
        }
      }


    public Task<bool> SetStatus(int id, string status)
      {
      if (!File.Exists(FilePath))
        {
        return Task.FromResult(false);
        }

      var taskFromJson = GetTaskFromJson();

      if (taskFromJson.Result.Count > 0)
        {
        var taskToBeUpdated = taskFromJson.Result
            .Where(task => task.Id == id)
            .SingleOrDefault();

        if (taskToBeUpdated != null)
          {
          var updatedTask = new ToDoTask()
            {
            Id = id,
            Description = taskToBeUpdated.Description,
            CreatedAt = taskToBeUpdated.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
            Status = GetStatusToSet(status)
            };

          taskFromJson.Result.Remove(taskToBeUpdated);
          taskFromJson.Result.Add(updatedTask);
          UpdateJsonFile(taskFromJson);
          return Task.FromResult(true);
          }
        }
      return Task.FromResult(false);
      }

    private StatusEnum GetStatusToSet(string status)
      {
      switch (status)
        {
        case "mark-in-progress":
          return StatusEnum.in_progress;
        case "mark-done":
          return StatusEnum.done;
        case "mark-todo":
          return StatusEnum.toDo;
        default:
          return StatusEnum.toDo;
        }
      }

    private StatusEnum GetStatusToDisplay(string status)
      {
      switch (status)
        {
        case "in-progress":
          return StatusEnum.in_progress;
        case "done":
          return StatusEnum.done;
        case "todo":
          return StatusEnum.toDo;
        default:
          return StatusEnum.toDo;
        }
      }


    public Task<bool> UpdateTask(int id, string description)
      {
      if (!File.Exists(FilePath))
        {
        return Task.FromResult(false);
        }

      var tasksFromJson = GetTaskFromJson();

      if (tasksFromJson.Result.Count > 0)
        {
        var taskToBeUpdated = tasksFromJson.Result
            .Where(task => task.Id == id)
            .SingleOrDefault();

        if (taskToBeUpdated != null)
          {
          var updatedTask = new ToDoTask
            {
            Id = id,
            Description = description,
            CreatedAt = taskToBeUpdated.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
            Status = taskToBeUpdated.Status
            };

          tasksFromJson.Result.Remove(taskToBeUpdated);
          tasksFromJson.Result.Add(updatedTask);
          UpdateJsonFile(tasksFromJson);
          return Task.FromResult(true);
          }
        }

      return Task.FromResult(false);
      }

    private bool CreateFileIfNotExists()
      {
      try
        {
        if (!File.Exists(FilePath))
          {
          using (FileStream fileStream = File.Create(FilePath))
            {
            Console.WriteLine($"File {FileName} created successfully");
            }
          }
        return true;
        }
      catch (Exception ex)
        {
        Console.WriteLine($"File {FileName} creation failed. Error: " + ex.Message);
        return false;
        }
      }
    }
  }
