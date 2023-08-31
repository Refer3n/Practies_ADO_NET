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

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||");

            storage.ShowAllProductsInfo();

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||");

            storage.ShowAllProductTypes();

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||");

            storage.ShowAllSuppliers();

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||");

            storage.ShowProductWithMaxQuantity();

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||");

            storage.ShowProductWithMinQuantity();

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||");

            storage.ShowProductWithMinCost();

            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||");

            storage.ShowProductWithMaxCost();
        }
    }

    public class Storage
    {
        public static string ConnectionString => ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

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

        public void ShowAllProductsInfo()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "select P.ProductName, PT.TypeName, S.SupplierName, P.Quantity, P.Cost, P.SupplyDate " +
                               "from Products P " +
                               "join ProductTypes PT on P.ProductType_Id = PT.Id " +
                               "join Suppliers S on P.Supplier_Id = S.Id";

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, "ProductsTable");

                Console.WriteLine("Product Name\tProduct Type\tSupplier Name\t\t\tQuantity\tCost\t\tSupply Date");

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    Console.WriteLine($"{row["ProductName"]}\t\t{row["TypeName"]}\t\t{row["SupplierName"]}\t\t" +
                        $"{row["Quantity"]}\t\t{row["Cost"]}\t\t{row["SupplyDate"]}");
                }

                connection.Close();
            }

        }

        public void ShowAllProductTypes()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "select TypeName from ProductTypes";

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, "ProductTypes");

                Console.WriteLine("Product types: ");

                Print(dataSet, "TypeName");

                connection.Close();
            }
        }

        public void ShowAllSuppliers()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "select SupplierName from Suppliers";

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, "Suppliers");

                Console.WriteLine("Product suppliers: ");

                Print(dataSet, "SupplierName");

                connection.Close();
            }
        }

        public void ShowProductWithMaxQuantity()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "select top 1 P.ProductName, P.Quantity " +
                       "from Products P " +
                       "order by P.Quantity desc";

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string productName = (string)reader["ProductName"];
                    int quantity = (int)reader["Quantity"];

                    Console.WriteLine($"Product with Maximum Quantity: {productName}, Quantity: {quantity}");
                }

                connection.Close();
            }
        }

        public void ShowProductWithMinQuantity()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "select top 1 P.ProductName, P.Quantity " +
                       "from Products P " +
                       "order by P.Quantity asc";

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string productName = (string)reader["ProductName"];
                    int quantity = (int)reader["Quantity"];

                    Console.WriteLine($"Product with Minimum Quantity: {productName}, Quantity: {quantity}");
                }

                reader.Close();

                connection.Close();
            }
        }

        public void ShowProductWithMinCost()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "select top 1 P.ProductName, P.Cost " +
                               "from Products P " +
                               "order by P.Cost asc";

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string productName = (string)reader["ProductName"];
                    decimal cost = (decimal)reader["Cost"];

                    Console.WriteLine($"Product with Minimum Cost: {productName}, Cost: {cost:f2}");
                }

                reader.Close();

                connection.Close();
            }
        }

        public void ShowProductWithMaxCost()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "select top 1 P.ProductName, P.Cost " +
                               "from Products P " +
                               "order by P.Cost desc";

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string productName = (string)reader["ProductName"];
                    decimal cost = (decimal)reader["Cost"];

                    Console.WriteLine($"Product with Maximum Cost: {productName}, Cost: {cost:f2}");
                }

                reader.Close();

                connection.Close();
            }
        }

        private void Print(DataSet dataSet, string key)
        {
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                Console.WriteLine(row[key]);
            }
        }
    }
}