using System.Data;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Npgsql;
namespace PgsqlManagementApp;

public partial class AppServices
{
    public static List<string> GetDatabases(string connectionStringReg)
    {
        List<string> databases = new List<string>();
        try
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionStringReg))
            {
                connection.Open();
                string query = "SELECT datname FROM pg_database WHERE datistemplate = false;";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            databases.Add(reader.GetString(0));
                        }
                    }
                }
                connection.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while fetching databases: {e.Message}");
        }

        return databases;
    }
    public static void ConnectionToDb()
    {
        try
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                Console.WriteLine("Successfully Connected!");
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while connection to Database: {ex.Message}");
        }
    }
    public static void CreateDatabase(string connectionStringReg)
    {
        Console.WriteLine("Enter name for your DB:");
        string dbName = Console.ReadLine();
        string dbCreationQuery = $"CREATE DATABASE {dbName}";

        try
        {
            using (var connection = new NpgsqlConnection(connectionStringReg))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(dbCreationQuery,connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Database created successfully!");
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while creating Database: {ex.Message}.");
        }
    }
}