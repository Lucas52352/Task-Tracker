using TaskTracker.Enums;

namespace TaskTracker.Domain
  {
  public class ToDoTask
    {
    public int Id { get; set; }

    public string Description { get; set; }

    public StatusEnum Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
    }
  }
