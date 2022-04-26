using System;
using System.Threading.Tasks;

namespace qguaratBot
{
    class Program
    {
        static void Main()
        {
            Bot bot = new Bot();
            bot.MainSync().GetAwaiter().GetResult();
        }
    }
}