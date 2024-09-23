using System.Data;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace PgsqlManagementApp;

class Program
{
    public static void Main(string[] args)
    {
        bool answer = true;
        List<string> selectManu = new List<string>
         {
             "Connection to DB",
             "Exit"
         };
         while (answer)
         {
             int choise = AppServices.Selector(selectManu);
             Console.Clear();

             switch (choise)
             {
                 case 0:
                     Console.WriteLine("Host(localhost):");
                     string host = Console.ReadLine();
                     Console.WriteLine("Port(5432):");
                     string port = Console.ReadLine();
                     Console.WriteLine("User Id(postgres):");
                     string userId = Console.ReadLine();
                     Console.WriteLine("Password(YourPass)");
                     string dbPass = Console.ReadLine();
                     string connectionStringReg =
                         $"Host = {host};Port = {port};User Id = {userId}; Password = {dbPass}; Database = postgres;";
                     List<string> databases = AppServices.GetDatabases(connectionStringReg);

                     if (databases.Count > 0)
                     {
                         databases.Add("Create a new database...");
                         int selectedIndex = AppServices.Selector(databases);
                         
                         if (selectedIndex == databases.Count - 1)
                         {
                             Console.Clear();
                             AppServices.CreateDatabase(connectionStringReg);
                             databases = AppServices.GetDatabases(connectionStringReg);
                             string chosenDatabase = databases[selectedIndex];
                             
                             AppServices.ConnectionString=$"Host={host};Username={userId};Password={dbPass};Port={port};Database={chosenDatabase}";
                             AppServices.ConnectionToDb();
                             Console.ReadKey();
                             
                         }
                         else
                         {
                             Console.Clear();
                             string chosenDatabase = databases[selectedIndex];
                             
                             AppServices.ConnectionString=$"Host={host};Username={userId};Password={dbPass};Port={port};Database={chosenDatabase}";
                             AppServices.ConnectionToDb();
                             Console.ReadKey();
                         }
                         Console.Clear();
                         List<string> crudTables = new List<string>
                         {
                             "Create",
                             "List Of Tables",
                             "Exit"
                         };
                         
                         while (true)
                         {
                             Console.Clear();
                             int choiseTabAct = AppServices.Selector(crudTables);

                             switch (choiseTabAct)
                             {
                                 case 0:
                                     Console.WriteLine("Enter name of table to create:");
                                     string tableName = Console.ReadLine();
                                     Console.WriteLine("Enter columns\n(Id SERIAL PRIMARY KEY, Name VARCHAR(100), Age INT)\nUse the sample above:");
                                     string tableColumns = Console.ReadLine();
                                     AppServices.CreateTable(tableName,tableColumns);
                                     Console.ReadKey();
                                     break;
                                 case 1:
                                     List<string> tablesList = AppServices.GetTables();
                                     if (tablesList.Count > 0)
                                     {
                                         tablesList.Add("Exit");
                                         int selectedTable = AppServices.Selector(tablesList);

                                         if (selectedTable < tablesList.Count - 1)
                                         {
                                             Console.Clear();
                                             string choosenTable = tablesList[selectedTable];

                                             while (true)
                                             {
                                                 Console.Clear();
                                                 List<string> tableActions = new List<string>
                                                 {
                                                     "Delete",
                                                     "Delete(Cascade)",
                                                     "Update Table",
                                                     "Truncate Table",
                                                     "Scripts",
                                                     "View data",
                                                     "Query tool",
                                                     "Columns",
                                                     "Exit"
                                                 };

                                                 int tableActionsChoise = AppServices.Selector(tableActions);

                                                 switch (tableActionsChoise)
                                                 {
                                                     case 0:
                                                         Console.WriteLine($"Are you sure you want to delete table {choosenTable}?(type: yes/no):");
                                                         string deleteAns = Console.ReadLine();
                                                         if (deleteAns.ToLower() == "yes")
                                                         {
                                                             AppServices.DeleteTable(choosenTable);
                                                         }
                                                         Console.ReadKey();
                                                         break;
                                                     case 1:
                                                         Console.WriteLine($"Are you sure you want to delete table {choosenTable}?(type: yes/no):");
                                                         string cascadeTable = Console.ReadLine();
                                                         if (cascadeTable.ToLower() == "yes")
                                                         {
                                                             AppServices.DeleteCascadeTable(choosenTable);
                                                         }
                                                         Console.ReadKey();
                                                         break;
                                                     case 2:
                                                         Console.WriteLine("Enter columns\n(Id SERIAL PRIMARY KEY)\nUse the sample above:");
                                                         string updTableNewColumns = Console.ReadLine();
                                                         AppServices.UpdateTable(choosenTable,updTableNewColumns);
                                                         Console.ReadKey();
                                                         break;
                                                     case 3:
                                                         Console.WriteLine($"Are you sure you want to delete table {choosenTable}?(type: yes/no):");
                                                         string truncateTable = Console.ReadLine();
                                                         if (truncateTable.ToLower() == "yes")
                                                         {
                                                             AppServices.DeleteCascadeTable(choosenTable);
                                                         }
                                                         Console.ReadKey();
                                                         break;
                                                     case 5:
                                                         List<string> selectedData =
                                                             AppServices.SelectData(choosenTable);
                                                         foreach (var row in selectedData)
                                                         {
                                                             Console.WriteLine(row);
                                                         }
                                                         Console.WriteLine("If you want to return to previous menu type (ok) ");
                                                         string isok = Console.ReadLine();
                                                         if (isok.ToLower() == "ok"){break;}
                                                         Console.ReadKey();
                                                         break;
                                                     case 4:
                                                         
                                                         break;
                                                     case 6:
                                                         Console.WriteLine("Enter your query:");
                                                         string query = Console.ReadLine();
                                                         AppServices.QueryTool(query);
                                                         Console.ReadKey();
                                                         break;
                                                     case 7:
                                                         AppServices.GetColumNames(choosenTable);
                                                         Console.ReadKey();
                                                         break;
                                                     case 8:
                                                         return;
                                                 }
                                             }

                                         }
                                         break;
                                     }
                                     else
                                     {
                                         
                                         Console.Clear();
                                         Console.WriteLine("There's no tables, press any button to returt to previous manu.");
                                         Console.ReadKey();
                                     }
                                     break;
                                 case 2:
                                     return;
                             }
                         }
                         
                     }
                     else
                     {
                         Console.WriteLine("No databases found. Please check the connection string info and try again");
                         Console.Write("Press any key to continue");
                         Console.ReadKey();
                         Console.Clear();
                     }
                     break;
                 case 1:
                     Console.WriteLine("Thank you for using our app!");
                     return;
             }
         }
    }
}