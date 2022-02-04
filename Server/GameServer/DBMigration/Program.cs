using DbUp;
using System;
using System.Linq;
using System.Reflection;

namespace DBMigration
{
    class Program
    {
        static int Main(string[] args)
        {
            var connectionString =
                    args.FirstOrDefault()
                    ?? "Server=localhost;Database=songoftheknights;Uid=root;Pwd=123456;";

            var upgrader =
                DeployChanges.To
                    .MySqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
#if DEBUG
                Console.ReadLine();
#endif
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;
        }
    }
}
