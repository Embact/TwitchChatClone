using System.Collections.Concurrent;

namespace SignalR.Models
{
    public class Debug
    {
        private static int _padleft = 14;
        private static int frames = 1;
        #region Logging
        public static void Log(string title, ConsoleColor titleColor, string message, ConsoleColor messageColor = ConsoleColor.White)
        {
            int padding = _padleft;
            Console.ForegroundColor = titleColor;
            Console.Write("[" + $"{title}".PadLeft(padding, ' ') + "] ");
            Console.ResetColor();

            Console.ForegroundColor = messageColor;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void Log(string title, ConsoleColor titleColor, string user, string message, ConsoleColor messageColor = ConsoleColor.White)
        {
            int padding = _padleft;
            Console.ForegroundColor = titleColor;
            Console.Write("[" + $"{title}".PadLeft(padding, ' ') + "] ");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"[{user}] ");
            Console.ResetColor();

            Console.ForegroundColor = messageColor;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void Log(string title, ConsoleColor titleColor, string room, string user, string message, ConsoleColor messageColor = ConsoleColor.White)
        {
            int padding = _padleft;
            Console.ForegroundColor = titleColor;
            Console.Write("[" + $"{title}".PadLeft(padding, ' ') + "] ");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"[{room}`s_Room] ");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"[{user}] ");
            Console.ResetColor();

            Console.ForegroundColor = messageColor;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        #endregion


        public static void Status(Dictionary<string, Room> _rooms, ConcurrentDictionary<string, Queue<List<Message>>> _messageQueue)
        {
            // Save the current cursor position
            int currentLine = Console.CursorTop;
           
            // Move the cursor to the status line (e.g., line 10)
            if (currentLine > 0)
            {
                Console.SetCursorPosition(0, 0);
                // Clear the status line
                Console.WriteLine(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, 0);
            }

            // Print the status line
            Console.WriteLine(new string('–', Console.WindowWidth));
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[" + DateTime.Now.ToLongTimeString() + "] ");
            Console.ResetColor();
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write($"LIVE HEARTBEAT");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($" Rooms: ");
            Console.ResetColor();
            Console.Write($"{_rooms.Count} , ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"Connections: ");
            Console.ResetColor();
            Console.Write($"{_rooms.DistinctBy(s => s.Value.Connections.Values.Distinct()).Sum(s => s.Value.Connections.Count())} , ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"Timeouts: ");
            Console.ResetColor();
            Console.Write($"{_rooms.Sum(s => s.Value.Timeouts.Count())} , ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"Bans: ");
            Console.ResetColor();
            Console.Write($"{_rooms.Sum(s => s.Value.Bans.Count())} , ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"Messages: ");
            Console.ResetColor();
            Console.Write($"{_rooms.Sum(s => s.Value.Messages.Count())} , ");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"Wait: ");
            Console.ResetColor();
            Console.Write($"{_messageQueue.Sum(s => s.Value.Sum(d => d.Count))} , ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"Frame: ");
            Console.ResetColor();
            Console.WriteLine($"{frames}".PadRight(9 - frames.ToString().Length, ' '));
            Console.WriteLine(new string('–', Console.WindowWidth));

            // Ensure new lines are added after the status line
            if (currentLine <= 1) // Adjust based on the number of lines in your status update
            {
                currentLine = 3; // Position after the status update
            }
            frames++;
            Console.SetCursorPosition(0, currentLine);
        }
    }
}
