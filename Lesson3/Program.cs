using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.Data.SqlClient;

namespace Lesson3
{
    internal class Program
    {
        private static DbProviderFactory factory = null;
        private static string connectionString = ConfigurationManager.ConnectionStrings["Default"].ToString();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Select database\n1 - SqlServer\n2 - Oracle");

            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);

            string selection = Console.ReadLine();

            if (selection == "1")
            {
                factory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings["Default"].ProviderName);
            }
            else if (selection == "2")
            {
                factory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings["OracleDb"].ProviderName);
            }
            else
            {
                factory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings["Default"].ProviderName);
            }

            //await ReadDataAsync(factory);

            StudentMarks studentMarks = new(factory);

            await studentMarks.TryConnectAsync();

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||||");

            await studentMarks.GetAllInfo();

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||||");

            await studentMarks.GetStudentsNames();

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||||");

            await studentMarks.GetAllAvg();

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||||");

            await studentMarks.GetStudentsWithHigherAvg(4);

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||||");

            await studentMarks.AddStudentAsync("John Doe", "A", 5, "Math", "History");

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||||");

            await studentMarks.GetAllInfo();

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||||");

            await studentMarks.UpdateStudentAsync("John Doe", "B", 4, "Physics", "Literature");

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||||");

            await studentMarks.GetAllInfo();

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||||");

            await studentMarks.DeleteStudentAsync("John Doe");

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||||");

            await studentMarks.GetAllInfo();

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||||");

        }

        private static async Task ReadDataAsync(DbProviderFactory factory)
        {
            using (DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;

                await connection.OpenAsync();

                using (DbCommand command = factory.CreateCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "select * from Clients";

                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader["Name"]);
                        }
                    }
                }
            }
        }

        //private async Task<DataSet> NewMethod(string dbName)
        //{
        //    DbProviderFactory factory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings[dbName].ProviderName);

        //    using (DbConnection connection = factory.CreateConnection())
        //    {
        //        await connection.OpenAsync();

        //        using(DbCommand command = factory.CreateCommand())
        //        {
        //            command.Connection = connection;

        //            command.CommandText = "select * from Clients";

        //            using(DbDataReader reader = command.ExecuteReader())
        //            {
        //                while(reader.Read())
        //                {
        //                    Console.WriteLine(reader["Name"]);
        //                }
        //            }
        //        }
        //    }
        //    return null;
        //}

        public class StudentMarks
        {
            private static string connectionString = ConfigurationManager.ConnectionStrings["Default"].ToString();

            private DbProviderFactory factory;

            public StudentMarks(DbProviderFactory factory)
            {
                this.factory = factory;
            }

            public async Task TryConnectAsync()
            {
                try
                {
                    using (DbConnection connection = factory.CreateConnection())
                    {
                        connection.ConnectionString = connectionString;

                        await connection.OpenAsync();

                        Console.WriteLine("Connection successful");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            public async Task GetAllInfo()
            {
                string query = "select FullName, GroupName, AvgByYear, LowestAvgSubject, HighestAvgSubject from StudentMarksTable";

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var result = await ExecuteReadQueryAsync(query, "FullName", "GroupName", "AvgByYear", "LowestAvgSubject", "HighestAvgSubject");
                stopwatch.Stop();

                Console.WriteLine(result);
                Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds} ms");
            }

            public async Task GetStudentsNames()
            {
                string query = "select FullName from StudentMarksTable";

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var result = await ExecuteReadQueryAsync(query, "FullName");
                stopwatch.Stop();

                Console.WriteLine(result);
                Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds} ms");
            }

            public async Task GetAllAvg()
            {
                string query = "select AvgByYear from StudentMarksTable";

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var result = await ExecuteReadQueryAsync(query, "AvgByYear");
                stopwatch.Stop();

                Console.WriteLine(result);
                Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds} ms");
            }

            public async Task GetStudentsWithHigherAvg(int requiredAvg)
            {
                string query = $"select FullName from StudentMarksTable where AvgByYear >= {requiredAvg}";

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var result = await ExecuteReadQueryAsync(query, "FullName");
                stopwatch.Stop();

                Console.WriteLine(result);
                Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds} ms");
            }

            public async Task AddStudentAsync(string fullName, string groupName, int avgByYear, string lowestAvgSubject, string highestAvgSubject)
            {
                string query = $"insert into StudentMarksTable (FullName, GroupName, AvgByYear, LowestAvgSubject, HighestAvgSubject) " +
                               $"values ('{fullName}', '{groupName}', {avgByYear}, '{lowestAvgSubject}', '{highestAvgSubject}')";

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                await ExecuteNonQueryAsync(query);
                stopwatch.Stop();

                Console.WriteLine("Student successfully added.");
                Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds} ms");
            }

            public async Task UpdateStudentAsync(string fullName, string groupName, int avgByYear, string lowestAvgSubject, string highestAvgSubject)
            {
                string query = $"update StudentMarksTable set GroupName = '{groupName}', AvgByYear = {avgByYear}, " +
                               $"LowestAvgSubject = '{lowestAvgSubject}', HighestAvgSubject = '{highestAvgSubject}' " +
                               $"where FullName = '{fullName}'";

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                await ExecuteNonQueryAsync(query);
                stopwatch.Stop();

                Console.WriteLine("Data successfully updated.");
                Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds} ms");
            }

            public async Task DeleteStudentAsync(string fullName)
            {
                string query = $"delete from StudentMarksTable where FullName = '{fullName}'";

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                await ExecuteNonQueryAsync(query);
                stopwatch.Stop();

                Console.WriteLine("Student successfully deleted.");
                Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds} ms");
            }

            private async Task ExecuteNonQueryAsync(string query)
            {
                using (DbConnection connection = factory.CreateConnection())
                {
                    connection.ConnectionString = connectionString;

                    await connection.OpenAsync();

                    using (DbCommand command = factory.CreateCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = query;
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }

            private async Task<string> ExecuteReadQueryAsync(string query, params string[] columnNames)
            {
                string result = "";

                using (DbConnection connection = factory.CreateConnection())
                {
                    connection.ConnectionString = connectionString;

                    await connection.OpenAsync();

                    using (DbCommand command = factory.CreateCommand())
                    {
                        command.Connection = connection;

                        command.CommandText = query;

                        using (DbDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    foreach (string columnName in columnNames)
                                    {
                                        result += $"{columnName}: {reader[columnName]}, ";
                                    }
                                    result += Environment.NewLine;
                                }
                            }
                            else
                            {
                                result = "Nothing was found.";
                            }
                        }

                    }
                }

                return result;
            }
        }
    }
}