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
                     Console.Write("Host(default: localhost):");
                     string host = Console.ReadLine();
                     if (string.IsNullOrEmpty(host))
                     {
                         host = "::1";
                     }
                     Console.Write("Port(5432):");
                     string inputPort = Console.ReadLine();
                     int port;
                     if (string.IsNullOrEmpty(inputPort))
                     {
                         port = 5432; 
                     }
                     else if (int.TryParse(inputPort, out port))
                     {
                         Console.WriteLine($"Port is: {port}");
                     }
                     else
                     {
                         Console.WriteLine("Invalid input, port value can only be int.\nPlease try again!");
                         Console.ReadKey();
                         break;
                     }
                     Console.Write("User Id(postgres):");
                     string userId = Console.ReadLine();
                     if (string.IsNullOrEmpty(userId))
                     {
                         userId = "postgres";
                     }
                     Console.WriteLine("Password(YourPass)");
                     
                     string dbPass = AppServices.ReadPassword();
                     string connectionStringReg =
                         $"Host = {host};Port = {port};User Id = {userId}; Password = {dbPass}; Database = postgres;";
                     List<string> databases = AppServices.GetDatabases(connectionStringReg);

                     if (databases.Count > 0)
                     {
                         databases.Add("Create a new database...");
                         databases.Add("Exit");
                         

                         bool retAnsDb = true;
                         while (retAnsDb)
                         {

                             int selectedIndex = AppServices.Selector(databases);
                             if (selectedIndex == databases.Count - 2)
                             {
                                 Console.Clear();
                                 AppServices.CreateDatabase(connectionStringReg);
                                 databases = AppServices.GetDatabases(connectionStringReg);
                                 string chosenDatabase = databases[selectedIndex];

                                 AppServices.ConnectionString =
                                     $"Host={host};Username={userId};Password={dbPass};Port={port};Database={chosenDatabase}";
                                 AppServices.ConnectionToDb();
                                 Console.ReadKey();

                             }else if (selectedIndex == databases.Count - 1)
                             {
                                 Console.WriteLine("Are you sure you want to exit app(type: yes/no)");
                                 string exitAppAns = Console.ReadLine();
                                 if (exitAppAns.ToLower() == "yes")
                                 {
                                     Console.Clear();
                                     Console.WriteLine("Thank You For Using Our App!");
                                     return;
                                 }
                             }
                             else
                             {
                                 Console.Clear();
                                 string chosenDatabase = databases[selectedIndex];

                                 AppServices.ConnectionString =
                                     $"Host={host};Username={userId};Password={dbPass};Port={port};Database={chosenDatabase}";
                                 AppServices.ConnectionToDb();
                                 Console.ReadKey();
                             }

                             Console.Clear();
                             List<string> crudTables = new List<string>
                             {
                                 "Create",
                                 "List Of Tables",
                                 "Back"
                             };

                             bool retAnsOptions = true;
                             while (retAnsOptions)
                             {
                                 Console.Clear();
                                 int choiseTabAct = AppServices.Selector(crudTables);

                                 switch (choiseTabAct)
                                 {
                                     case 0:
                                         Console.WriteLine("Enter name of table to create:");
                                         string tableName = Console.ReadLine();
                                         Console.WriteLine(
                                             "Enter columns\n(Id SERIAL PRIMARY KEY, Name VARCHAR(100), Age INT)\nUse the sample above:");
                                         string tableColumns = Console.ReadLine();
                                         AppServices.CreateTable(tableName, tableColumns);
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

                                                 bool retAnsTableAction = true;
                                                 while (retAnsTableAction)
                                                 {
                                                     Console.Clear();
                                                     List<string> tableActions = new List<string>
                                                     {
                                                         $"Actions of table {choosenTable}:",
                                                         "Delete",
                                                         "Delete(Cascade)",
                                                         "Update Table",
                                                         "Truncate Table",
                                                         "Scripts",
                                                         "View data",
                                                         "Query tool",
                                                         "Columns",
                                                         "Get Table relations",
                                                         "Exit"
                                                         
                                                     };

                                                     int tableActionsChoise = AppServices.Selector(tableActions);

                                                     switch (tableActionsChoise)
                                                     {
                                                         case 1:
                                                             Console.WriteLine(
                                                                 $"Are you sure you want to delete table {choosenTable}?(type: yes/no):");
                                                             string deleteAns = Console.ReadLine();
                                                             if (deleteAns.ToLower() == "yes")
                                                             {
                                                                 AppServices.DeleteTable(choosenTable);
                                                             }

                                                             Console.ReadKey();
                                                             break;
                                                         case 2:
                                                             Console.WriteLine(
                                                                 $"Are you sure you want to delete table {choosenTable}?(type: yes/no):");
                                                             string cascadeTable = Console.ReadLine();
                                                             if (cascadeTable.ToLower() == "yes")
                                                             {
                                                                 AppServices.DeleteCascadeTable(choosenTable);
                                                             }

                                                             Console.ReadKey();
                                                             break;
                                                         case 3:
                                                             Console.WriteLine(
                                                                 "Enter columns\n(Id SERIAL PRIMARY KEY)\nUse the sample above:");
                                                             string updTableNewColumns = Console.ReadLine();
                                                             AppServices.UpdateTable(choosenTable, updTableNewColumns);
                                                             Console.ReadKey();
                                                             break;
                                                         case 4:
                                                             Console.WriteLine(
                                                                 $"Are you sure you want to delete table {choosenTable}?(type: yes/no):");
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

                                                             Console.WriteLine(
                                                                 "If you want to return to previous menu type (ok) ");
                                                             string isok = Console.ReadLine();
                                                             if (isok.ToLower() == "ok")
                                                             {
                                                                 break;
                                                             }

                                                             Console.ReadKey();
                                                             break;
                                                         case 6:

                                                             break;
                                                         case 7:
                                                             List<string> querryToolSelect = new List<string>
                                                             {
                                                                 "DDL, DML data",
                                                                 "Read only",
                                                                 "Back"
                                                             };

                                                             bool ansQuery = true;
                                                             while (ansQuery)
                                                             {
                                                                 int optionQuery =
                                                                     AppServices.Selector(querryToolSelect);

                                                                 switch (optionQuery)
                                                                 {
                                                                     case 0:
                                                                         Console.WriteLine("Enter your query:");
                                                                         string queryDml = Console.ReadLine();
                                                                         AppServices.QueryToolDml(queryDml);
                                                                         Console.ReadKey();
                                                                         break;
                                                                     case 1:
                                                                         Console.WriteLine("Enter your query:");
                                                                         string readQuery = Console.ReadLine();
                                                                         AppServices.QueryToolDml(readQuery);
                                                                         Console.ReadKey();
                                                                         break;
                                                                     case 2:
                                                                         ansQuery = false;
                                                                         break;
                                                                 }
                                                                 
                                                             }
                                                             break;
                                                         case 8:
                                                             AppServices.GetColumNames(choosenTable);
                                                             Console.ReadKey();
                                                             break;
                                                         case 9:
                                                             Console.Clear();
                                                             AppServices.GetTableRelationships(choosenTable);
                                                             Console.WriteLine("\n\nPress any button to returt to previous manu");
                                                             Console.ReadKey();
                                                             break;
                                                         case 10:
                                                             retAnsTableAction = false;
                                                             break;
                                                     }
                                                 }

                                             }

                                             break;
                                         }
                                         else
                                         {

                                             Console.Clear();
                                             Console.WriteLine(
                                                 "There's no tables, press any button to returt to previous manu.");
                                             Console.ReadKey();
                                         }
                                         break;
                                     case 2:
                                         retAnsOptions = false;
                                         break;
                                 }
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