using System;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FactoryProvader
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            string providerName;
            string connectionString;

            while (true)
            {
                Console.WriteLine("Выберите СУБД: ");
                Console.WriteLine("1 SQL Server");
                Console.WriteLine("2 SQLite");
                Console.WriteLine("0 Выход");

                string dbChoice = Console.ReadLine();
                if (dbChoice == "0")
                {
                    return;
                }
                if (dbChoice == "1")
                {
                    providerName = "System.Data.SqlClient";
                    connectionString = "Server=Arsen;Database=VegetablesFruits;Trusted_Connection=True;";
                }
                else if (dbChoice == "2")
                {
                    providerName = "System.Data.SQlite";
                    connectionString = "Data Source=VegetablesFruits.db;";
                }
                else
                {
                    Console.WriteLine("Error");
                    continue;
                }
                try
                {
                    DbProviderFactory factory = DbProviderFactories.GetFactory(providerName);

                    using (DbConnection connection = factory.CreateConnection())
                    {
                        connection.ConnectionString = connectionString;
                        await connection.OpenAsync();

                        await ShowProductsAsync(connection);

                        Console.WriteLine("\nВведите ID для обновления: ");
                        if (int.TryParse(Console.ReadLine(), out int NewId))
                        {
                            Console.WriteLine("Новое имя продукта: ");
                            string NewName = Console.ReadLine();
                            await UpdateProductsAsync(connection, NewId, NewName);
                        }
                        else
                        {
                            Console.WriteLine("Error");
                        }
                        await ShowProductsAsync(connection);

                        Console.WriteLine("\nВведите ID для удаления: ");
                        if (int.TryParse(Console.ReadLine(), out int DelId))
                        {
                            await DeleteProductAsync(connection, DelId);
                        }
                        else
                        {
                            Console.WriteLine("Error");
                        }

                        await ShowProductsAsync(connection);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        static async Task ShowProductsAsync(DbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM VegFruTable";

                var sec = Stopwatch.StartNew();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    Console.WriteLine("\nСписок продуктов:");
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine($"ID: {reader["ID"]}, Название: {reader["Name"]}, Тип: {reader["Type"]}, Калории: {reader["Calories"]}");
                    }
                }
                sec.Stop();
                Console.WriteLine($"Время выполнения: {sec.Elapsed.TotalSeconds:F2} секунд");
            }
        }

        static async Task UpdateProductsAsync(DbConnection connection, int id, string newName)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE VegFruTable SET Name = @name WHERE ID = @id";

                var ParamName = command.CreateParameter();
                ParamName.ParameterName = "@name";
                ParamName.Value = newName;
                command.Parameters.Add(ParamName);

                var ParamId = command.CreateParameter();
                ParamId.ParameterName = "@id";
                ParamId.Value = id;
                command.Parameters.Add(ParamId);

                var sec = Stopwatch.StartNew();
                int rows = await command.ExecuteNonQueryAsync();
                sec.Stop();
                Console.WriteLine($"Обновлено: {rows}.\nВремя: {sec.Elapsed.TotalSeconds:F2} секунд");
            }
        }

        static async Task DeleteProductAsync(DbConnection connection, int id)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM VegFruTable WHERE ID = @id";

                var ParamId = command.CreateParameter();
                ParamId.ParameterName = "@id";
                ParamId.Value = id;
                command.Parameters.Add(ParamId);

                var sec = Stopwatch.StartNew();
                int rows = await command.ExecuteNonQueryAsync();
                sec.Stop();
                Console.WriteLine($"Удалено: {rows}.\nВремя: {sec.Elapsed.TotalSeconds:F2} секунд");
            }
        }
    }
}