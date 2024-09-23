using System.Data;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Npgsql;
namespace PgsqlManagementApp;

public partial class AppServices
{
    public static string ConnectionString { get; set; }

    public static int Selector(List<string> choise)
    {
        int indexSelection = 0;
        while (true)
        {
            Console.Clear();
            for (int i = 0; i < choise.Count; i++)
            {
                if (i == indexSelection)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Beep();
                    Console.WriteLine(choise[i]);
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"{choise[i]}");
                    Console.ResetColor();
                }
            }
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.DownArrow) 
                indexSelection = (indexSelection + 1) % choise.Count;
            else if (key.Key == ConsoleKey.UpArrow)
                indexSelection = (indexSelection - 1 + choise.Count)%choise.Count;
            else if (key.Key == ConsoleKey.Enter) 
                return indexSelection;
        }
    }
}