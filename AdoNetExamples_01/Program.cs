
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

internal class Program
{
    private static async Task Main(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
         .SetBasePath(Directory.GetCurrentDirectory())
         .AddJsonFile("appsettings.json")
         .Build();

        string server_connectionString = configuration.GetConnectionString("MsSqlConnectionString") ?? "DefaultConnectionString";
        string databaseName = "Library-ADO-NET-Demo";

        #region Connection Info
        Console.WriteLine("============================================");
        using (SqlConnection connection = new(server_connectionString))
        {
            await connection.OpenAsync();
            Console.WriteLine("Connection is open");
            Console.WriteLine("Connection properties:");
            Console.WriteLine($"Connection string: {connection.ConnectionString}");
            Console.WriteLine($"Database: {connection.Database}");
            Console.WriteLine($"Host: {connection.DataSource}");
            Console.WriteLine($"Host version: {connection.ServerVersion}");
            Console.WriteLine($"State: {connection.State}");
            Console.WriteLine($"Workstationld: {connection.WorkstationId}");
        }
        #endregion


        #region Create / Update / Delete Data
        Console.WriteLine("============================================");
        using (SqlConnection connection = new(server_connectionString))
        {
            await connection.OpenAsync();
            SqlCommand command = new();
            command.CommandText = $"CREATE DATABASE {databaseName}";
            command.Connection = connection;
            await command.ExecuteNonQueryAsync();
        }

            //creating database connection string
            var builder = new SqlConnectionStringBuilder(server_connectionString)
            {
                InitialCatalog = databaseName
            };
            string database_conectionstring = builder.ToString();


        using (SqlConnection db_connection = new(database_conectionstring))
        {
            await db_connection.OpenAsync();
            SqlCommand command = new();
            command.CommandText = @"CREATE TABLE Books (
                            Id INT PRIMARY KEY IDENTITY,
                            Title NVARCHAR(100) NOT NULL,
                            Author NVARCHAR(100) NOT NULL,
                            PageCount INT NOT NULL)";
            command.Connection = db_connection;
            await command.ExecuteNonQueryAsync();
            Console.WriteLine("TABLE Books is created!");
        }

        using (SqlConnection db_connection = new(database_conectionstring))
        {
            await db_connection.OpenAsync();
            SqlCommand command = new(
                @"INSERT INTO Books (Title, Author, PageCount) VALUES
                            ('To Kill a Mockingbird', 'Harper Lee', 150),
                            ('1984', 'George Orwell', 98),
                            ('The Great Gatsby', 'F. Scott Fitzgerald', 321)", db_connection);
            int number = await command.ExecuteNonQueryAsync();
            Console.WriteLine($"Added objects count: {number}");
        }

        using (SqlConnection db_connection = new(database_conectionstring))
        {
            await db_connection.OpenAsync();
            SqlCommand command = new(@"UPDATE Books SET PageCount=120 WHERE Title='1984'", db_connection);
            int number = await command.ExecuteNonQueryAsync();
            Console.WriteLine($"Updated objects count: {number}");
        }

        using (SqlConnection db_connection = new(database_conectionstring))
        {
            await db_connection.OpenAsync();
            SqlCommand command = new(@"DELETE FROM Books WHERE Title='1984'", db_connection);
            int number = await command.ExecuteNonQueryAsync();
            Console.WriteLine($"Deleted objects count: {number}");
        }
        #endregion


        #region Read Data
        Console.WriteLine("============================================");
        using (SqlConnection connection = new(database_conectionstring))
        {
            await connection.OpenAsync();

            SqlCommand command = new("SELECT * FROM Books", connection);
            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                if (reader.HasRows)
                {
                    int columnWidth = 20;
                    string columnName1 = reader.GetName(0);
                    string columnName2 = reader.GetName(1);
                    string columnName3 = reader.GetName(2);
                    string columnName4 = reader.GetName(3);

                    Console.WriteLine($"{columnName1.PadRight(columnWidth)} \t{columnName2.PadRight(columnWidth)} \t{columnName3.PadRight(columnWidth)} \t{columnName4.PadRight(columnWidth)} ");

                    while (await reader.ReadAsync())
                    {
                        int id = reader.GetInt32(0);
                        string title = reader.GetString(1);
                        string author = reader.GetString(2);
                        int pageCount = reader.GetInt32(3);

                        Console.WriteLine($"{id.ToString().PadRight(columnWidth)} \t{title.PadRight(columnWidth)} \t{author.PadRight(columnWidth)} \t{pageCount.ToString().PadRight(columnWidth)}");
                    }
                }
            }
        }

        using (SqlConnection connection = new(database_conectionstring))
        {
            await connection.OpenAsync();

            SqlCommand command = new("SELECT COUNT(*) FROM Books", connection);
            object count = await command.ExecuteScalarAsync();

            command.CommandText = "SELECT MIN(PageCount) FROM Books";
            object minBook = await command.ExecuteScalarAsync();

            Console.WriteLine($"Sum objects count: {count}");
            Console.WriteLine($"Min Page Count: {minBook}");
        }
        #endregion


        #region Parameterized Queries
        Console.WriteLine("============================================");
        string sqlExpression = "INSERT INTO Books (Title, Author, PageCount) VALUES (@title, @author, @pageCount);SET @id=SCOPE_IDENTITY()";
        using (SqlConnection connection = new(database_conectionstring))
        {
            await connection.OpenAsync();

            SqlCommand command = new(sqlExpression, connection);

            SqlParameter titleParam = new("@title", "Jane Eyre");
            command.Parameters.Add(titleParam);

            SqlParameter authorParam = new("@author", "Charlotte Brontë");
            command.Parameters.Add(authorParam);

            SqlParameter pageCountParam = new("@pageCount", 215);
            command.Parameters.Add(pageCountParam);

            SqlParameter idParam = new()
            {
                ParameterName = "@id",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(idParam);

            int number = await command.ExecuteNonQueryAsync();
            Console.WriteLine($"Added objects count: {number}");
            Console.WriteLine($"New book Id: {idParam.Value}");
        }
        #endregion


        #region Transactions
        Console.WriteLine("============================================");
        using (SqlConnection connection = new(database_conectionstring))
        {
            await connection.OpenAsync();
            SqlTransaction transaction = connection.BeginTransaction();

            SqlCommand command = connection.CreateCommand();
            command.Transaction = transaction;

            try
            {
                command.CommandText = "INSERT INTO Books (Title, Author, PageCount) VALUES ('Book1', 'Author1', 5)";
                await command.ExecuteNonQueryAsync();
                command.CommandText = "INSERT INTO Books (Title, Author, PageCount) VALUES ('Book2', 'Author2', 55)";
                await command.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await transaction.RollbackAsync();
            }
        }
        #endregion

        Console.Read();
    }
}