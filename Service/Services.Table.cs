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
        Console.Clear();
        List<string> data = new List<string>();
        string query = $"SELECT * FROM {tableName};";
        try
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                DataTable dataTable = new DataTable();
                using (var command = new NpgsqlCommand(query, connection)) 
                using (var reader = command.ExecuteReader())
                {
                    dataTable.Load(reader);
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        Console.WriteLine($"{column.ColumnName} : {row[column]}, ");
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
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader.GetString("Column_Name"));
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
    }
    public static void QueryToolDml(string query)
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

    public static void QueryToolRead(string query)
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

    public static void GetTableRelationships(string tableName)
    {
        var relationships = new List<TableRelationship>();
        var primaryKeys = new List<string>();

        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Open();
        
            string primaryKeyQuery = @"
                SELECT 
                    a.attname AS column_name
                FROM 
                    pg_index i
                JOIN 
                    pg_attribute a ON a.attnum = ANY(i.indkey) AND a.attrelid = i.indrelid
                WHERE 
                    i.indrelid = @tableName::regclass
                    AND i.indisprimary;";

            using (var command = new NpgsqlCommand(primaryKeyQuery, connection))
            { 
                command.Parameters.AddWithValue("tableName", tableName);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        primaryKeys.Add(reader["column_name"].ToString());
                    }
                }
            }
        
            string foreignKeyQuery = @"
                SELECT 
                    conname AS constraint_name, 
                    conrelid::regclass::text AS table_from, 
                    confrelid::regclass::text AS table_to,
                    a.attname AS from_column,
                    af.attname AS to_column
                FROM 
                    pg_constraint AS c 
                 JOIN 
                    pg_attribute AS a ON a.attnum = ANY(c.conkey) AND a.attrelid = c.conrelid
                JOIN 
                    pg_attribute AS af ON af.attnum = ANY(c.confkey) AND af.attrelid = c.confrelid
                WHERE 
                    c.contype = 'f'
                    AND (conrelid::regclass::text = @tableName OR confrelid::regclass::text = @tableName);";

            using (var command = new NpgsqlCommand(foreignKeyQuery, connection))
            {
                command.Parameters.AddWithValue("tableName", tableName);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var relationship = new TableRelationship
                        {
                            ConstraintName = reader["constraint_name"].ToString(),
                            TableFrom = reader["table_from"].ToString(),
                            FromColumn = reader["from_column"].ToString(),
                            TableTo = reader["table_to"].ToString(),
                            ToColumn = reader["to_column"].ToString()
                        };

                        relationships.Add(relationship);
                    }
                }
            }
        }

        Console.WriteLine($"Primary Keys for table '{tableName}':");
        foreach (var primaryKey in primaryKeys)
        {
            Console.WriteLine($"- {primaryKey}");
        }

        Console.WriteLine();
   
        Console.WriteLine($"\nForeign Key Relationships for table '{tableName}':");
        foreach (var relationship in relationships)
        {
            Console.WriteLine($"Constraint: {relationship.ConstraintName}, Table From: {relationship.TableFrom}, Column: {relationship.FromColumn} -> Table To: {relationship.TableTo}, Column: {relationship.ToColumn}");
        }
    }

}