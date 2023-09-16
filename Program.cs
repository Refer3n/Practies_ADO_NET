using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Pract1
{
    public class Program
    {
        static void Main(string[] args)
        {
            Storage storage = new Storage();

            storage.TryConnect();

            storage.FillDB();

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||");

            storage.ShowAllItems();

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||");

            storage.ShowAllTypes();

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||");

            storage.ShowAllSuppliers();
        }
    }

    public class Storage
    {
        private static string ConnectionString => ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

        private static string GetSelectQuery(string tbName) => $"select * from {tbName}";

        Dictionary<string, Dictionary<string, List<string>>> database;

        public Storage()
        {
            database = new();
        }

        public void TryConnect()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    Console.WriteLine("Connection successful");

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void ShowAllItems()
        {
            foreach (var tableName in database.Keys)
            {
                Console.WriteLine($"Table: {tableName}");
                var tableData = database[tableName];

                if (tableData.Count > 0)
                {
                    var columnNames = tableData.Keys.ToList();

                    Console.WriteLine(string.Join("\t", columnNames));

                    var rowCount = tableData[columnNames[0]].Count;

                    for (int i = 0; i < rowCount; i++)
                    {
                        foreach (var columnName in columnNames)
                        {
                            Console.Write(tableData[columnName][i] + "\t  ");
                        }
                        Console.WriteLine();
                    }
                }

                Console.WriteLine();
            }
        }


        public void ShowAllTypes()
        {
            if (database.ContainsKey("[dbo].[ProductTypes]"))
            {
                var types = database["[dbo].[ProductTypes]"]["TypeName"];
                Console.WriteLine("Types of Products:");
                Console.WriteLine(string.Join(", ", types));
            }
        }

        public void ShowAllSuppliers()
        {
            if (database.ContainsKey("[dbo].[Suppliers]"))
            {
                var types = database["[dbo].[Suppliers]"]["SupplierName"];
                Console.WriteLine("Suppliers: ");
                Console.WriteLine(string.Join(", ", types));
            }
        }

        public void FillDB()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    var tableNames = GetTableNames(connection);

                    foreach (var tableName in tableNames)
                    {
                        var columns = GetColumnNames(connection, tableName);

                        var tableData = new Dictionary<string, List<string>>();

                        using (SqlCommand cmd = new SqlCommand(GetSelectQuery(tableName), connection))
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    foreach (var columnName in columns)
                                    {
                                        if (!tableData.ContainsKey(columnName))
                                        {
                                            tableData[columnName] = new List<string>();
                                        }
                                        tableData[columnName].Add(reader[columnName].ToString());
                                    }
                                }
                            }
                        }

                        database[tableName] = tableData;
                    }

                    Console.WriteLine("Database filled successfully");

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static List<string> GetTableNames(SqlConnection sqlConnection, params string[] name)
        {
            var tables = new List<string>();

            if (name.Length > 0)
            {
                foreach (DataRow dr in sqlConnection.GetSchema("Tables").Rows)
                {
                    if (name.Contains(dr[2].ToString()))
                    {
                        tables.Add($"[{dr[1]}].[{dr[2]}]");
                    }
                }
            }
            else
            {
                foreach (DataRow dr in sqlConnection.GetSchema("Tables").Rows)
                {
                    tables.Add($"[{dr[1]}].[{dr[2]}]");
                }
            }

            return tables;
        }

        private static List<string> GetColumnNames(SqlConnection sqlConnection, string tbName)
        {
            var colums = new List<string>();

            using (SqlCommand cmd = new SqlCommand(GetSelectQuery(tbName), sqlConnection))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        colums.Add(reader.GetName(i));
                    }
                }
            }
            return colums;
        }
    }
}