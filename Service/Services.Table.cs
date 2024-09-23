using System.Data;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Npgsql;
namespace PgsqlManagementApp;

public partial class AppServices
{
    public static List<string> GetTables()
    {
        List<string> tables = new List<string>();
        try
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tables.Add(reader.GetString(0));
                        }
                    }
                }
                connection.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while fetching tables: {e.Message}");
        }

        return tables;
    }
    public static void CreateTable(string tableName,string tableColumns)
    {
        string query = "CREATE TABLE IF NOT EXISTS "+tableName+" " +tableColumns+";";
        try
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query,connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Successfully Created.");
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while creating table{ex.Message}");
        }
    }
    public static void DeleteTable(string tableName)
    {
        string query = "DROP TABLE " + tableName;
        try
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query,connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Successfully DELETED.");
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while deleting table: {ex.Message}.");
        }
    }
    public static void DeleteCascadeTable(string tableName)
    {
        string query = "TRUNCATE TABLE " + tableName;
        try
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query,connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Successfully DELETED.");
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while deleting table: {ex.Message}.");
        }
    }
    public static void UpdateTable(string tableName, string tableColumns)
    {
        string query = $"ALTER TABLE "+tableName+"\n ADD COLUMN "+tableColumns+";";
        try
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query,connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Successfully Created.");
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while creating table{ex.Message}");
        }
    }
    public static List<string> SelectData(string tableName)
    {
        List<string> data = new List<string>();
        string query = $"SELECT * FROM {tableName};";
        try
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query,connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(reader.GetString(0));
                        }
                    }
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while creating table{ex.Message}");
        }

        return data;
    }
    public static void GetColumNames(string tableName)
    {
        string query = "SELECT colum_name FROM USER_TAB_COLUMN WHERE table_name = '"+tableName+"'";
        try
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query,connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Successfully Created.");
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while creating table{ex.Message}");
        }
    }
    public static void QueryTool(string query)
    {
        try
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query,connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Success!");
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while deleting table: {ex.Message}.");
        }
    }

}