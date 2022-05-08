namespace qguaratBot
{
    public static class Console
    {
        private static readonly object consoleLock = new object();

        public enum LogLevel
        {
            INFO,
            // DEBUG,
            WARNING,
            ERROR,
        }

        public static void Log(LogLevel logLevel, string logText)
        {
            lock (consoleLock)
            {
                // System.Console.WriteLine($"[{DateTime.Now}]\t[{logLevel}]\t{logText}");

                System.Console.Write($"[{DateTime.Now}]\t");

                if (logLevel is LogLevel.INFO) System.Console.ForegroundColor = ConsoleColor.Blue;
                if (logLevel is LogLevel.WARNING) System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                if (logLevel is LogLevel.ERROR) System.Console.ForegroundColor = ConsoleColor.Red;

                System.Console.Write($"[{logLevel}]\t");

                System.Console.ResetColor();
                
                System.Console.Write($"{logText}");

                System.Console.WriteLine();
            }
        }
    }
}