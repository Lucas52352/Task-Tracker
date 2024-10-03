using System.Text.RegularExpressions;

namespace TaskTracker.Utils
  {
  public static class ConsolePrintConfig
    {
    public static void PrintInfoMsg(string message)
      {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("\n" + message);
      Console.ResetColor();
      }

    public static void PrintHelpMsg(string message)
      {
      Console.ForegroundColor = ConsoleColor.Magenta;
      Console.WriteLine("\n" + message);
      Console.ResetColor();
      }

    public static void PrintErrorMsg(string message)
      {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("\n" + message);
      Console.ResetColor();
      }

    public static void PrintCommandMsg(string message)
      {
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine("\n" + message);
      Console.ResetColor();
      }

    public static void PrintNumberMsg(string message)
      {
      Console.ForegroundColor = ConsoleColor.Blue;
      Console.WriteLine("\n" + message);
      Console.ResetColor();
      }

    public static List<string> ParseInput(string input)
      {
      var cmdArgs = new List<string>();

      var regex = new Regex(@"[\""].+?[\""]|[^ ]+");
      var matches = regex.Matches(input);

      foreach (Match match in matches)
        {
        string value = match.Value.Trim();
        cmdArgs.Add(value);
        }
      return cmdArgs;
      }

    public static void ClearConsole()
      {
      Console.Clear();
      }

    }
  }
