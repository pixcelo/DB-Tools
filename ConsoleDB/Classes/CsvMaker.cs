using System.Text;

namespace ConsoleDB.Classes
{
    public static class CsvMaker
    {
        /// <summary>
        /// 顧客テーブルのテストデータをCSVファイルに出力
        /// </summary>
        /// <param name="rows">作成するテストデータのレコード数</param>
        /// <returns></returns>
        public static async Task CreateCsvAsync(int rows)
        {
            var csvPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads",
                "test.csv");

            // ファイルを初期化
            File.WriteAllText(csvPath, string.Empty);

            const int bufferSize = 1024 * 1024; // 1MB buffer
            using var writer = new StreamWriter(csvPath, false, Encoding.UTF8, bufferSize);
            var csv = new StringBuilder(bufferSize);
            var random = new Random();

            for (int i = 0; i < rows; i++)
            {
                var id = i + 1;
                var name = Guid.NewGuid().ToString("N").Substring(0, 8);
                var email = $"{Guid.NewGuid().ToString("N").Substring(0, 8)}@test.com";
                var date = new DateTime(
                    random.Next(2000, 2022),
                    random.Next(1, 13),
                    random.Next(1, 29),
                    random.Next(0, 24),
                    random.Next(0, 60),
                    random.Next(0, 60)
                );

                csv.Append(id)
                   .Append(',')
                   .Append(name)
                   .Append(',')
                   .Append(email)
                   .Append(',')
                   .Append(date.ToString("yyyy-MM-dd HH:mm:ss"))
                   .AppendLine();

                if (csv.Length >= bufferSize / 2)
                {
                    await writer.WriteAsync(csv);
                    csv.Clear();
                }
            }

            if (csv.Length > 0)
            {
                await writer.WriteAsync(csv);
            }
        }
    }
}
