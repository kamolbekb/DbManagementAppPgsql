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
                    Console.Beep();
                    Console.ForegroundColor = ConsoleColor.Cyan;
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
    
    public static string ReadPassword()
    {
        string password = string.Empty;
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(intercept: true); // Read the key without displaying it

            // If the key is not Enter, add the character to the password
            if (key.Key != ConsoleKey.Enter&&key.Key!=ConsoleKey.Backspace)
            {
                // Append the character to the password
                password += key.KeyChar;
                // Write an asterisk to the console
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password.Substring(0, password.Length - 1);
                Console.Write("\b \b");
            }


        } while (key.Key != ConsoleKey.Enter); // Continue until Enter is pressed

        return password;
    }
}