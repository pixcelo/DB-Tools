using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

while (true)
{
    Console.WriteLine("モデルクラスを生成するテーブル名を入力してください (終了するには 'exit' と入力): ");
    var tableName = Console.ReadLine();

    if (tableName?.ToLower() == "exit")
    {
        break;
    }

    //var templateFolder = Path.Combine(Directory.GetCurrentDirectory());
    //var relativePath = Path.GetRelativePath(templateFolder, @"C:\Users\ModelTemplate.tt");

    var templatePath = "..\\..\\..\\Templates\\ModelTemplate.tt";

    if (File.Exists(templatePath))
    {
        // T4テンプレートファイルの内容を読み込む
        string templateContent = File.ReadAllText(templatePath);

        // テーブルのカラム情報を取得
        string columns = GetTableColumns(tableName);

        // テーブル名をテンプレート内のプレースホルダに置換
        templateContent = templateContent.Replace("TableName", tableName);
        templateContent = templateContent.Replace("Columns", columns);

        // テンプレートファイルを書き換え
        var outputPath = "..\\..\\..\\Models\\" + tableName + ".cs";
        File.WriteAllText(outputPath, templateContent);

        // T4テンプレートの実行（Visual Studio上での自動実行が一般的）        
        Console.WriteLine($"モデルクラスの生成が完了しました。{Environment.NewLine}{Path.GetFullPath(outputPath)} に保存されました。");
        Console.WriteLine();
        Console.WriteLine();
    }
    else
    {
        Console.WriteLine("T4テンプレートファイルが見つかりません。");
    }
}

// カラム情報を取得する
static string GetTableColumns(string tableName)
{
    var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

    string connectionString = configuration.GetConnectionString("SQLServerConnection");
    string columns = "";

    using (var connection = new SqlConnection(connectionString))
    {
        connection.Open();
        string query = @$"
            SELECT 
                COLUMN_NAME,
                DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS
            WHERE
                TABLE_NAME = '{tableName}'";

        using (var command = new SqlCommand(query, connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                string columnName = reader["COLUMN_NAME"].ToString();
                string dataType = reader["DATA_TYPE"].ToString();
                columns += $"public {MapDataType(dataType)} {columnName} {{ get; set; }}{Environment.NewLine}";
            }
        }

    }
    return columns;
}

// DBのデータ型とC#のデータ型のマッピング
static string MapDataType(string sqlDataType)
{
    var DataTypeMap = new Dictionary<string, string>
    {
        { "int", "int" },
        { "bigint", "long" },
        { "decimal", "decimal" },
        { "nvarchar", "string" },
        { "varchar", "string" },
        { "char", "string" },
        { "bid", "bool" },
        { "datetime", "DateTime" },
    };

    const string defaultDataType = "string";

    if (DataTypeMap.TryGetValue(sqlDataType, out var csharpDataType))
    {
        return csharpDataType;
    }
    return defaultDataType;
}