using TaskTracker.Domain;

namespace TaskTracker.Interfaces
  {
  public interface ITaskService
    {
    Task<List<ToDoTask>> GetAllTasks();

    Task<List<ToDoTask>> GetTaskByStatus(string status);

    Task<int> AddNewTask(string description);

    Task<bool> UpdateTask(int id, string description);

    Task<bool> DeleteTask(int id);

    Task<bool> SetStatus(int id, string status);

    List<string> GetHelpCommands();
    }
  }
