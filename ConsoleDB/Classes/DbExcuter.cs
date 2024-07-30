using Dapper;
using System.Data.SqlClient;

namespace ConsoleDB.Classes
{
    public static class DbExcuter
    {
        private static string connectionString　= 
            "Server=(localdb)\\MSSQLLocalDB;Database=sampleDb1;Integrated Security=True;";

        public static void Insert()
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();

                var query = "INSERT INTO Customers VALUES ('Tom' ,'test@gmail.com', '2024-07-26')";
                _ = connection.Execute(query);
            }
        }

        /// <summary>
        /// 1000件ずつインサート
        /// </summary>
        public static void StepInsert()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var csvPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads",
                    "test.csv");

                var lines = File.ReadAllLines(csvPath);
                var batchSize = 1000;
                var batch = new List<string>();

                foreach (var line in lines)
                {
                    batch.Add(line);
                    if (batch.Count == batchSize)
                    {
                        InsertBatch(connection, batch);
                        batch.Clear();
                    }
                }

                if (batch.Count > 0)
                {
                    InsertBatch(connection, batch);
                }
            }
        }

        /// <summary>
        /// 100万件までインサートできることを確認
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="batch"></param>
        private static void InsertBatch(SqlConnection connection, List<string> batch)
        {
            var query = "INSERT INTO Customers (CustomerId, CustomerName, Email, RegisterdDate) VALUES ";
            var values = new List<string>();

            foreach (var line in batch)
            {
                var columns = line.Split(',');
                var value = $"({columns[0]}, '{columns[1]}', '{columns[2]}', '{columns[3]}')";
                values.Add(value);
            }

            query += string.Join(", ", values);

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    using (var command = new SqlCommand(query, connection, transaction))
                    {
                        command.CommandTimeout = 600; // タイムアウトを延長
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public static void BulkInsert()
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 1200;
                connection.Open();

                var csvPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads",
                    "test.csv");

                var query = $"BULK INSERT Customers FROM '{csvPath}' WITH ( FORMAT = 'CSV' );";
                _ = connection.Execute(query);
            }
        }

        public static void Execute(string query)
        {

        }
    }
}
